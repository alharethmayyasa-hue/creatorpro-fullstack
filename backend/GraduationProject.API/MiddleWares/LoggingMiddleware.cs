using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GraduationProject.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var path = context.Request.Path.ToString();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var elapsed = stopwatch.ElapsedMilliseconds;
                var statusCode = context.Response.StatusCode;

                int threshold = path.Contains("/ai/", StringComparison.OrdinalIgnoreCase)
                    ? 15000
                    : 1000;

                bool isExternalSystemDown = statusCode == 502 || statusCode == 503 || statusCode == 504;
                bool isServerError = statusCode >= 500 && !isExternalSystemDown;
                bool isSlowRequest = elapsed > threshold;

                if (isServerError || isSlowRequest || isExternalSystemDown)
                {
                    string category = isExternalSystemDown ? "ExternalSystem"
                        : isServerError ? "InternalError"
                        : "Performance";

                    var logLevel = isServerError ? LogLevel.Error : LogLevel.Warning;

                    _logger.Log(
                        logLevel,
                        "🚨 {Method} {Path} | Status: {StatusCode} | Time: {Elapsed}ms | Category: {Category}",
                        context.Request.Method,
                        path,
                        statusCode,
                        elapsed,
                        category
                    );
                }
            }
        }
    }
}
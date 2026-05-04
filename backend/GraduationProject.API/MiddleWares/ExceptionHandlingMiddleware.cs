using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GraduationProject.Application.Common.Exceptions;
using GraduationProject.Application.Common.Responses;

namespace GraduationProject.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // 1. تحديد كود الحالة أولاً
                int statusCode = ex is AppException appEx ? appEx.StatusCode : 500;

                // 2. تسجيل فقط أخطاء السيرفر (5xx) مع تفاصيل الطلب لتسهيل التشخيص
                if (statusCode >= 500)
                {
                    var method = context.Request.Method.Replace("\r", "").Replace("\n", "");
                    var path = context.Request.Path.ToString().Replace("\r", "").Replace("\n", "");
                    var traceId = context.TraceIdentifier.Replace("\r", "").Replace("\n", "");
                    _logger.LogError(
                        ex,
                        "Server Error: {Message} | {Method} {Path} | TraceId: {TraceId}",
                        ex.Message,
                        method,
                        path,
                        traceId);
                }

                await HandleExceptionAsync(context, ex, statusCode);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            string message = "An unexpected error occurred.";
            IDictionary<string, string[]>? errors = null;

            if (exception is AppException appEx)
            {
                message = appEx.Message;

                if (appEx is AppValidationException valEx)
                {
                    errors = valEx.Errors;
                }
            }
            else if (_env.IsDevelopment())
            {
                message = exception.Message;
            }

            var response = ApiResponse<object>.Fail(message, errors);

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
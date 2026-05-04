using GraduationProject.API.Attributes.Subscription;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using GraduationProject.Application.Contracts.Services;

namespace GraduationProject.API.Middlewares.Subscription
{
    public class SubscriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public SubscriptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISubscriptionService subService)
        {
            var endpoint = context.GetEndpoint();

            var attribute = endpoint?.Metadata.GetMetadata<RequireActiveSubscriptionAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { Message = "Unauthorized: Please log in first." });
                return;
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { Message = "Invalid user data" });
                return;
            }

            var isValid = await subService.IsSubscriptionValidAsync(userId);

            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                await context.Response.WriteAsJsonAsync(new { Message = "Access Denied: Your subscription is expired or inactive." });
                return;
            }

            await _next(context);
        }
    }
}


using System.Linq;
using GraduationProject.Application.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GraduationProject.API.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                        );

                    var response = ApiResponse<object>.Fail(
                        "Validation failed",
                        errors
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
/*
 * Summary
 * This extension configures a custom API behavior that standardizes how validation
 * errors are returned across the entire application. Instead of ASP.NET Core's
 * default ModelState error format, this implementation converts all validation
 * failures into a unified ApiResponse<object>.Fail structure.
 *
 *  What It Does:
 * - Intercepts invalid ModelState before the controller executes.
 * - Extracts all validation errors from ModelState.
 * - Converts them into a dictionary:  { "FieldName": ["Error1", "Error2"] }
 * - Wraps the errors inside ApiResponse<object>.Fail("Validation failed", errors)
 * - Returns a clean, predictable 400 BadRequest response.
 *
 *  How It Works:
 * - ASP.NET Core automatically triggers InvalidModelStateResponseFactory
 *   whenever a request fails validation (e.g., missing required fields).
 * - This extension overrides the default behavior and ensures the response
 *   always follows the same structure used by the rest of the API.
 *
 * Purpose:
 * - Ensures all validation errors across all controllers follow a consistent format.
 * - Eliminates the need for manual ModelState checks inside controllers.
 * - Makes frontend integration easier by guaranteeing predictable error responses.
 *
 * Example Output:
 *   {
 *      "isSuccess": false,
 *      "message": "Validation failed",
 *      "data": null,
 *      "errors": {
 *          "Email": ["The Email field is required."],
 *          "Password": ["Password must be at least 6 characters."]
 *      }
 *   }
 *
 * How to Use:
 * - Register this extension in Program.cs or Startup.cs:
 *
 *      services.AddCustomApiBehavior();
 *
 * - After registration, all DTO validation errors will automatically return
 *   the unified ApiResponse format without writing any extra code in controllers.
 *
 * This extension is a key part of the API infrastructure, ensuring clean,
 * consistent, and professional error handling across the entire backend.
 */

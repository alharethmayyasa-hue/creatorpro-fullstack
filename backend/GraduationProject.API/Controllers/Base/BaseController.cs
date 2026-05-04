using Microsoft.AspNetCore.Mvc;
using GraduationProject.Application.Common.Responses;
using System.Security.Claims;

namespace GraduationProject.API.Controllers.Base
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
          protected int? CurrentUserId =>
           int.TryParse(User?.FindFirst("userId")?.Value, out var id) ? id : null;

       protected string? CurrentUserEmail =>
           User?.FindFirst(ClaimTypes.Email)?.Value;

       protected bool IsOwner(int userId) => CurrentUserId == userId;

       
        // =========================
        // ✅ Success
        // =========================
        protected IActionResult Success<T>(T data, string? message = "Success")
            => Ok(ApiResponse<T>.Success(data, message));

        protected IActionResult CreatedResponse<T>(T data, string? message = "Created")
            => StatusCode(201, ApiResponse<T>.Success(data, message));

        protected IActionResult PagedResponse<T>(
            IEnumerable<T> items,
            int page,
            int pageSize,
            int totalCount)
        {
            var paged = new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(ApiResponse<PagedResult<T>>.Success(paged));
        }

        // =========================
        // ❌ Errors
        // =========================
        protected IActionResult Fail(
            string message,
            IDictionary<string, string[]>? errors = null,
            int statusCode = 400)
            => StatusCode(statusCode, ApiResponse<object>.Fail(message, errors));

        protected IActionResult NotFoundResponse(string message = "Not Found")
            => NotFound(ApiResponse<object>.Fail(message));

        protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
            => Unauthorized(ApiResponse<object>.Fail(message));

        protected IActionResult NoContentResponse()
            => NoContent();
    }
}
/*
 * Summary
 * This BaseController provides a unified set of helper methods that standardize
 * how all API endpoints return responses across the entire application.
 * It ensures that every controller inherits consistent behavior for success,
 * error handling, and pagination formatting.
 *
 * 1) Success Responses
 *    - Success<T>(data, message)
 *         Returns a 200 OK response wrapped inside ApiResponse<T>.
 *    - CreatedResponse<T>(data, message)
 *         Returns a 201 Created response for newly created resources.
 *    - PagedResponse<T>(items, page, pageSize, totalCount)
 *         Wraps paginated data inside PagedResult<T> and returns a unified ApiResponse.
 *
 * 2) Error Responses
 *    - Fail(message, errors, statusCode)
 *         Returns a structured error response using ApiResponse<object>.Fail.
 *    - NotFoundResponse(message)
 *         Returns a 404 Not Found with a unified error format.
 *    - UnauthorizedResponse(message)
 *         Returns a 401 Unauthorized with a unified error format.
 *    - NoContentResponse()
 *         Returns a 204 No Content when no body is required.
 *
 * Purpose:
 * The BaseController eliminates repetitive boilerplate code in all controllers.
 * It enforces a consistent API response structure, improves maintainability,
 * and ensures that frontend clients always receive predictable JSON responses.
 *
 * How to Use in Your Controllers:
 * - Make your controller inherit from BaseController:
 *
 *      public class UserController : BaseController
 *
 * - Then use the built‑in helpers instead of Ok(), BadRequest(), NotFound(), etc.:
 *
 *      return Success(userDto);
 *      return CreatedResponse(newUser);
 *      return Fail("Invalid request");
 *      return NotFoundResponse("User not found");
 *      return UnauthorizedResponse("Token expired");
 *
 * - For paginated endpoints:
 *
 *      return PagedResponse(items, page, pageSize, totalCount);
 *
 * These helpers ensure that all responses follow the ApiResponse<T> format,
 * and integrate seamlessly with the ExceptionHandlingMiddleware.
 */

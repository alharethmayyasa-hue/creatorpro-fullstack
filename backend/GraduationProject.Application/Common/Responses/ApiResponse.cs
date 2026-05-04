using System.Collections.Generic;

namespace GraduationProject.Application.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public IDictionary<string, string[]>? Errors { get; init; }

        private ApiResponse(bool isSuccess, string? message, T? data, IDictionary<string, string[]>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> Success(T data, string? message = null)
            => new(true, message, data);

        public static ApiResponse<T> Fail(string message, IDictionary<string, string[]>? errors = null)
            => new(false, message, default, errors);
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; init; } =  Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
    }
}
/*
 * Summary
 * This file defines two core response models used to standardize all API outputs:
 *
 * 1) ApiResponse<T>
 *    - Provides a unified structure for every API response, whether successful or failed.
 *    - Includes:
 *        • IsSuccess : Indicates whether the operation succeeded.
 *        • Message   : A descriptive message about the result.
 *        • Data      : The returned payload (if any).
 *        • Errors    : Validation or business errors (if any).
 *    - Offers helper factory methods:
 *        • Success() : Creates a successful response with optional message.
 *        • Fail()    : Creates a failure response with optional error dictionary.
 *    - Ensures consistent JSON output across all controllers and endpoints.
 *
 * 2) PagedResult<T>
 *    - Represents paginated data returned from list endpoints.
 *    - Includes:
 *        • Items      : The items in the current page.
 *        • Page       : Current page number.
 *        • PageSize   : Number of items per page.
 *        • TotalCount : Total number of items in the dataset.
 *    - Typically wrapped inside ApiResponse<T> for consistent API formatting.
 *
 * Purpose:
 * These models enforce a clean, predictable, and uniform API response structure,
 * making the backend easier to maintain and the frontend easier to integrate with.
 *
 *  How to Use in Your Code:
 * - Inside your controllers, never return Ok(), BadRequest(), or NotFound() directly.
 * - Instead, use the helper methods provided by BaseController:
 *
 *      return Success(userDto);
 *      return Fail("Invalid request");
 *      return CreatedResponse(createdItem);
 *
 * - For paginated endpoints:
 *
 *      return PagedResponse(items, page, pageSize, totalCount);
 *
 * - For validation or business errors thrown from services:
 *      throw new BadRequestException("Invalid data");
 *      throw new NotFoundException("User not found");
 *
 *   These exceptions will automatically be converted into ApiResponse<T>.Fail
 *   by the ExceptionHandlingMiddleware.
 *
 * This ensures that **every single API endpoint** returns the same consistent structure,
 * without repeating boilerplate code in each controller.
 */

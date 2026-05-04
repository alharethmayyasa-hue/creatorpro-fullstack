using System;
using System.Collections.Generic;

namespace GraduationProject.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, 404) { }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, 400) { }
    }

    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, 401) { }
    }

    public class AppValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public AppValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed", 400)
        {
            Errors = errors;
        }
    }
}
/*
 *  Summary
 * This file defines a hierarchy of custom application exceptions used to
 * represent business, validation, and authorization errors in a clean and
 * structured way.
 *
 *  What It Contains:
 * 1) AppException (abstract)
 *    - Base class for all custom exceptions.
 *    - Stores an HTTP status code and a message.
 *
 * 2) NotFoundException
 *    - Represents 404 Not Found errors.
 *
 * 3) BadRequestException
 *    - Represents 400 Bad Request errors.
 *
 * 4) UnauthorizedException
 *    - Represents 401 Unauthorized errors.
 *
 * 5) AppValidationException
 *    - Represents validation errors with a dictionary of field-specific messages.
 *
 *  How It Works:
 * - Services throw these exceptions when something goes wrong:
 *
 *      throw new NotFoundException("User not found");
 *      throw new BadRequestException("Invalid input");
 *      throw new AppValidationException(errors);
 *
 * - The ExceptionHandlingMiddleware catches them and converts them into
 *   ApiResponse<object>.Fail with the correct status code.
 *
 * Purpose:
 * - Keeps business logic clean by avoiding manual status code handling.
 * - Ensures consistent error messages across the entire API.
 * - Makes debugging easier by using meaningful exception types.
 *
 * Usage:
 * - Throw these exceptions inside services instead of returning null or bool.
 * - The middleware will automatically handle them and return a unified response.
 */

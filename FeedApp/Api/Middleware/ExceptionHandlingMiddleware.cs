using FeedApp.Application.DTOs.Common;
using System.Diagnostics;
using FeedApp.Domain.Exceptions;

namespace FeedApp.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                logger.LogWarning(ex, "Application error: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
                await HandleAppExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occurred");
                await HandleUnhandledExceptionAsync(context, ex);
            }
        }

        private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
        {
            context.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ForbiddenException => StatusCodes.Status403Forbidden,
                ConflictException => StatusCodes.Status409Conflict,
                ValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ErrorResponse
            {
                ErrorCode = ex.ErrorCode,
                Message = ex.Message,
                Details = ex is ValidationException validationEx && validationEx.Errors.Count > 0
                    ? validationEx.Errors
                    : null,
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandleUnhandledExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new ErrorResponse
            {
                ErrorCode = "INTERNAL_ERROR",
                Message = "An unexpected error occurred. Please try again later.",
                TraceId = Activity.Current?.Id ?? context.TraceIdentifier
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

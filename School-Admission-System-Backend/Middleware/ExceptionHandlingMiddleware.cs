using FluentValidation;
using School_Admission_System_Backend.Shared;
using System.Text.Json;

namespace School_Admission_System_Backend.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                await WriteJsonAsync(context, ex.StatusCode, ex.Message);
            }
            catch (ValidationException ex)
            {
                var message = ex.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.";
                await WriteJsonAsync(context, StatusCodes.Status400BadRequest, message);
            }
            catch (Exception)
            {
                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        // All error responses share the same simple shape: { "message": "..." }.
        private static async Task WriteJsonAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
        }
    }
}

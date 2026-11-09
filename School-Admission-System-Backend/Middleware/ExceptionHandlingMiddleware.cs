using FluentValidation;
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
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                var response = JsonSerializer.Serialize(new { Message = "Validation Failed", Errors = errors });

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(response);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}

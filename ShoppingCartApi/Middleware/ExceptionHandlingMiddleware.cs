using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ShoppingCartApi.Domain.Exceptions;

namespace ShoppingCartApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";
            var errors = new Dictionary<string, string[]>();

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = validationException.Message;
                    errors = (Dictionary<string, string[]>)validationException.Errors;
                    break;
                // Puedes añadir más tipos de excepciones personalizadas aquí
                default:
                    // Para otras excepciones no manejadas, se mantiene el error 500
                    // En producción, se debería loggear el error completo pero no exponerlo al cliente.
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                statusCode = (int)statusCode,
                message = message,
                errors = errors.Any() ? errors : null
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

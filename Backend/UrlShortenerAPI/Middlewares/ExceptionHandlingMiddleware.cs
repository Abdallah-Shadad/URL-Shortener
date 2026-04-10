using System.Net;
using System.Text.Json;

namespace UrlShortenerAPI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {

        private static Task WriteErrorResponse(HttpContext context, string errorMessage, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = errorMessage });

            return context.Response.WriteAsync(result);
        }

        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // pass to next middleware
            }
            catch (KeyNotFoundException ex)
            {
                // 404 Not Found
                await WriteErrorResponse(context, ex.Message, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 403 Forbidden
                await WriteErrorResponse(context, ex.Message, HttpStatusCode.Forbidden);
            }
            catch (InvalidOperationException ex)
            {
                // 400 Bad Reqasuest
                await WriteErrorResponse(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                // 500 Internal Server Error 
                await WriteErrorResponse(context, ex.Message, HttpStatusCode.InternalServerError);
            }
        }
    }
}
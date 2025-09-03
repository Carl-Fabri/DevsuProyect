using System.Net;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace DevsuBackend.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int status;
            string errorCode;

            switch (exception)
            {
                case UnauthorizedAccessException:
                case SecurityTokenException:
                    status = (int)HttpStatusCode.Unauthorized;
                    errorCode = "unauthorized";
                    break;
                case ArgumentException:
                case InvalidOperationException:
                    status = (int)HttpStatusCode.BadRequest;
                    errorCode = "bad_request";
                    break;
                case KeyNotFoundException:
                    status = (int)HttpStatusCode.NotFound;
                    errorCode = "not_found";
                    break;
                default:
                    status = (int)HttpStatusCode.InternalServerError;
                    errorCode = "server_error";
                    break;
            }

            var response = new
            {
                error = errorCode,
                message = exception.Message,
                traceId = context.TraceIdentifier
            };

            var payload = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;
            return context.Response.WriteAsync(payload);
        }
    }
}

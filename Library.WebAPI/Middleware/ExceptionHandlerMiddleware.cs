using System.Net;
using System.Text.Json;

namespace Library.WebAPI.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            var response = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = "Internal Server Error from the custom middleware."
            };

            if (exception is CustomException customException)
            {
                statusCode = customException.StatusCode;
                response.Message = customException.Message;

                if (customException is ValidationFailedException validationException)
                {
                    response.Errors = validationException.Errors;
                }
            }
            else if (exception is FluentValidation.ValidationException validationException)
            {
                statusCode = HttpStatusCode.BadRequest;
                response.Message = "Validation Failed";
                response.Errors = validationException.Errors.Select(e => e.ErrorMessage);
            }
            else if (exception is UnauthorizedAccessException unauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
                response.Message = unauthorizedAccessException.Message;
            }

            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

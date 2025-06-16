using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SmartApp.Shared.Common;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace SmartApp.WebApi.Middleware
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class AppValidationException : Exception
    {
        public List<string> Errors { get; }

        public AppValidationException(List<string> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }
    }

    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next,
                                         ILogger<GlobalExceptionMiddleware> logger,
                                         IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var traceId = context.TraceIdentifier;
            int statusCode;
            string errorMessage;
            object errorDetails = null;

            switch (exception)
            {
                case AppValidationException appValidationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = "Validation errors occurred.";
                    errorDetails = appValidationEx.Errors;
                    break;

                case ValidationException dataAnnotationsEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    errorMessage = "Validation failed.";
                    errorDetails = new List<string> { dataAnnotationsEx.Message };
                    break;

                case NotFoundException notFoundEx:
                    statusCode = (int)HttpStatusCode.NotFound;
                    errorMessage = notFoundEx.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    errorMessage = _env.IsDevelopment()
                        ? $"{exception.Message}"
                        : "An unexpected error occurred. Please contact support.";
                    if (_env.IsDevelopment())
                    {
                        errorDetails = new
                        {
                            ExceptionType = exception.GetType().Name,
                            StackTrace = exception.StackTrace,
                            InnerException = exception.InnerException?.Message
                        };
                    }
                    break;
            }

            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

            context.Response.StatusCode = statusCode;

            var response = new Response<object>
            {
                isSuccess = false,
                message = $"{errorMessage} (Trace ID: {traceId})",
                data = errorDetails
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _env.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}

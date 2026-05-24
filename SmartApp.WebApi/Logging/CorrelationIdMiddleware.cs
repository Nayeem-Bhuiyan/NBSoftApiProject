using Serilog.Context;

namespace SmartApp.WebApi.Logging;

public sealed class CorrelationIdMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
                         ?? Guid.NewGuid().ToString();

        // ← push to response so client can trace
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items["CorrelationId"]                = correlationId;

        // ← push to Serilog context for all log entries in this request
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
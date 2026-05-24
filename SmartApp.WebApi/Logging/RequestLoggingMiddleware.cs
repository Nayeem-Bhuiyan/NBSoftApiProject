using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;

namespace SmartApp.WebApi.Logging;

public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LogSettings _settings;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        IOptions<LogSettings> settings,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next     = next;
        _settings = settings.Value;
        _logger   = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_settings.EnableRequestLogging)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var userName = context.User.FindFirstValue(ClaimTypes.Name)           ?? "anonymous";
        var clientIp = ResolveClientIp(context);
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? string.Empty;

        // ← enrich all logs within this request scope
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("UserName", userName))
        using (LogContext.PushProperty("ClientIp", clientIp))
        using (LogContext.PushProperty("UserAgent", userAgent))
        using (LogContext.PushProperty("RequestMethod", method))
        using (LogContext.PushProperty("RequestPath", path))
        {
            string? requestBody = null;
            string? responseBody = null;

            // ← read request body if needed
            if (_settings.EnableRequestLogging && IsLoggableContentType(context.Request.ContentType))
                requestBody = await ReadRequestBodyAsync(context);

            // ← buffer response if LogResponseBody attribute present
            var logResponseBody = HasLogResponseBodyAttribute(context);
            Stream? originalBody = null;
            MemoryStream? responseBuffer = null;

            if (logResponseBody || ShouldLogErrorResponse(context))
            {
                responseBuffer          = new MemoryStream();
                originalBody            = context.Response.Body;
                context.Response.Body   = responseBuffer;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                var durationMs = stopwatch.ElapsedMilliseconds;

                // ← read buffered response for error responses (4xx/5xx) always
                if (responseBuffer is not null && originalBody is not null)
                {
                    responseBuffer.Seek(0, SeekOrigin.Begin);
                    responseBody = await new StreamReader(responseBuffer).ReadToEndAsync();

                    responseBuffer.Seek(0, SeekOrigin.Begin);
                    await responseBuffer.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;
                }

                LogRequest(
                    method, path, statusCode, durationMs,
                    requestBody, responseBody,
                    correlationId, logResponseBody);
            }
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private void LogRequest(
        string method,
        string path,
        int statusCode,
        long durationMs,
        string? requestBody,
        string? responseBody,
        string correlationId,
        bool logResponseBody)
    {
        var maskedRequest = requestBody is not null
            ? SensitiveDataMasker.MaskJson(requestBody, _settings.SensitiveFields)
            : null;

        var maskedResponse = responseBody is not null && (logResponseBody || statusCode >= 400)
            ? SensitiveDataMasker.MaskJson(responseBody, _settings.SensitiveFields)
            : null;

        var logLevel = statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };

        _logger.Log(logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {DurationMs}ms | " +
            "CorrelationId: {CorrelationId} | RequestBody: {RequestBody} | ResponseBody: {ResponseBody}",
            method, path, statusCode, durationMs,
            correlationId, maskedRequest, maskedResponse);
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return body;
    }

    private static bool IsLoggableContentType(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType)) return false;
        return contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
               contentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasLogResponseBodyAttribute(HttpContext context)
    {
        return context.GetEndpoint()?.Metadata
            .OfType<LogResponseBodyAttribute>()
            .Any() ?? false;
    }

    private static bool ShouldLogErrorResponse(HttpContext context) => true;

    private static string ResolveClientIp(HttpContext context)
    {
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        return forwarded?.Split(',').FirstOrDefault()?.Trim()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";
    }
}

public sealed class LogSettings
{
    public const string SectionName = "LogSettings";

    public bool EnableRequestLogging { get; init; } = true;
    public bool EnableResponseLogging { get; init; } = false;
    public string LogFilePath { get; init; } = "logs/smartapp-.log";
    public int RetainedFileCount { get; init; } = 31;
    public string MsSqlTableName { get; init; } = "ApplicationLogs";
    public List<string> SensitiveFields { get; init; } = new();
}
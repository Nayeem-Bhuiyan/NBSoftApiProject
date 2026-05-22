using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using SmartApp.Application.Interfaces.RateLimit;
using SmartApp.Shared.Common;

namespace SmartApp.WebApi.RateLimit;

public sealed class RateLimitFilter : IAsyncActionFilter
{
    private readonly IRateLimitService _rateLimitService;
    private readonly RateLimitOptions _options;
    private readonly ILogger<RateLimitFilter> _logger;

    public RateLimitFilter(
        IRateLimitService rateLimitService,
        IOptions<RateLimitOptions> options,
        ILogger<RateLimitFilter> logger)
    {
        _rateLimitService = rateLimitService;
        _options          = options.Value;
        _logger           = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // ← check DisableRateLimiting first — method level takes priority
        if (HasDisableAttribute(context))
        {
            await next();
            return;
        }

        if (!_options.Enabled)
        {
            await next();
            return;
        }

        var policyName = ResolvePolicy(context);
        if (policyName is null)
        {
            await next();
            return;
        }

        if (!_options.Policies.TryGetValue(policyName, out var policy))
        {
            _logger.LogWarning("Rate limit policy '{Policy}' not found in configuration.", policyName);
            await next();
            return;
        }

        var key = ResolveKey(context, policy.KeyType);
        var result = await _rateLimitService.IsAllowedAsync(
            policyName, key, policy.PermitLimit, policy.WindowSeconds);

        // ← append rate limit headers to every response
        context.HttpContext.Response.Headers["X-RateLimit-Limit"]     = policy.PermitLimit.ToString();
        context.HttpContext.Response.Headers["X-RateLimit-Remaining"] = result.RemainingRequests.ToString();
        context.HttpContext.Response.Headers["X-RateLimit-Reset"]     = policy.WindowSeconds.ToString();

        if (!result.IsAllowed)
        {
            _logger.LogWarning(
                "Rate limit exceeded. Policy: {Policy}, Key: {Key}, Count: {Count}",
                policyName, key, result.TotalRequests);

            context.HttpContext.Response.Headers["Retry-After"] = result.RetryAfterSeconds.ToString();
            context.HttpContext.Response.StatusCode             = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType            = "application/json";

            var response = Response<string>.Failure(
                $"Too many requests. Please retry after {result.RetryAfterSeconds} seconds.");

            await context.HttpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

            context.Result = new EmptyResult();
            return;
        }

        await next();
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static bool HasDisableAttribute(ActionExecutingContext context)
    {
        // ← method-level check first
        var actionDisabled = context.ActionDescriptor.EndpointMetadata
            .OfType<DisableRateLimitingAttribute>()
            .Any();

        return actionDisabled;
    }

    private static string? ResolvePolicy(ActionExecutingContext context)
    {
        // ← method-level attribute takes priority over controller-level
        var actionAttr = context.ActionDescriptor.EndpointMetadata
            .OfType<RateLimitPolicyAttribute>()
            .LastOrDefault();

        return actionAttr?.PolicyName;
    }

    private static string ResolveKey(ActionExecutingContext context, string keyType)
    {
        return keyType.ToLower() switch
        {
            "user" => ResolveUserKey(context),
            "apikey" => ResolveApiKey(context),
            _ => ResolveIpKey(context)   // default: IP
        };
    }

    private static string ResolveIpKey(ActionExecutingContext context)
    {
        // ← respect X-Forwarded-For for proxy/load balancer scenarios
        var forwarded = context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ip = forwarded?.Split(',').FirstOrDefault()?.Trim()
                     ?? context.HttpContext.Connection.RemoteIpAddress?.ToString()
                     ?? "unknown";
        return ip;
    }

    private static string ResolveUserKey(ActionExecutingContext context)
    {
        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId)) return userId;

        // ← fallback to IP if user not authenticated
        return ResolveIpKey(context);
    }

    private static string ResolveApiKey(ActionExecutingContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-Api-Key"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(apiKey)) return apiKey;

        return ResolveIpKey(context);
    }
}
namespace SmartApp.Application.Interfaces.RateLimit;

public interface IRateLimitService
{
    /// <summary>
    /// Returns true if request is allowed, false if rate limit exceeded.
    /// </summary>
    Task<RateLimitResult> IsAllowedAsync(
        string policyName,
        string key,
        int permitLimit,
        int windowSeconds,
        CancellationToken ct = default);
}

public sealed record RateLimitResult(
    bool IsAllowed,
    int RemainingRequests,
    int TotalRequests,
    long RetryAfterSeconds
);
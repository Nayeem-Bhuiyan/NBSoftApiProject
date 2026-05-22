using Microsoft.Extensions.Caching.Distributed;
using SmartApp.Application.Interfaces.RateLimit;
using StackExchange.Redis;

namespace SmartApp.Infrastructure.Services.RateLimit;

public sealed class RedisRateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisRateLimitService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<RateLimitResult> IsAllowedAsync(
        string policyName,
        string key,
        int permitLimit,
        int windowSeconds,
        CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var redisKey = $"ratelimit:{policyName}:{key}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var windowStart = now - windowSeconds;

        // ← sliding window using Redis sorted set
        var transaction = db.CreateTransaction();

        // remove expired entries outside the window
        _ = transaction.SortedSetRemoveRangeByScoreAsync(redisKey, 0, windowStart);

        // count current requests in window
        var countTask = transaction.SortedSetLengthAsync(redisKey);

        // add current request with timestamp as score
        _ = transaction.SortedSetAddAsync(redisKey, $"{now}:{Guid.NewGuid()}", now);

        // set expiry on the key
        _ = transaction.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(windowSeconds + 1));

        await transaction.ExecuteAsync();

        var currentCount = (int)await countTask + 1;
        var isAllowed = currentCount <= permitLimit;
        var remaining = Math.Max(0, permitLimit - currentCount);

        // ← calculate retry-after only when exceeded
        long retryAfter = 0;
        if (!isAllowed)
        {
            var oldest = await db.SortedSetRangeByScoreAsync(
                redisKey, double.NegativeInfinity, double.PositiveInfinity,
                take: 1);

            if (oldest.Length > 0 && double.TryParse(oldest[0].ToString().Split(':')[0], out var oldestScore))
                retryAfter = (long)(oldestScore + windowSeconds - now);
        }

        return new RateLimitResult(
            IsAllowed: isAllowed,
            RemainingRequests: remaining,
            TotalRequests: currentCount,
            RetryAfterSeconds: retryAfter
        );
    }
}
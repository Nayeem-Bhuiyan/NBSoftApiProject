using SmartApp.Application.Interfaces.RateLimit;
using SmartApp.Infrastructure.Services.RateLimit;
using SmartApp.WebApi.RateLimit;
using StackExchange.Redis;

namespace SmartApp.WebApi.Extensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RateLimitOptions>(
            configuration.GetSection(RateLimitOptions.SectionName));

        // ← reuse existing Redis connection if registered, else register
        if (!services.Any(s => s.ServiceType == typeof(IConnectionMultiplexer)))
        {
            var redisConnection = configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException("Redis connection string not configured.");

            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnection));
        }

        services.AddScoped<IRateLimitService, RedisRateLimitService>();
        services.AddScoped<RateLimitFilter>();

        return services;
    }
}
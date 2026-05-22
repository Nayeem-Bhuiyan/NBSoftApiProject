namespace SmartApp.WebApi.RateLimit;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public bool Enabled { get; init; } = true;
    public Dictionary<string, RateLimitPolicy> Policies { get; init; } = new();
}

public sealed class RateLimitPolicy
{
    public int PermitLimit { get; init; } = 100;
    public int WindowSeconds { get; init; } = 60;
    public string KeyType { get; init; } = "IP";  // IP | User | ApiKey
}
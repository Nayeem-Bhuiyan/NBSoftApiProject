// Configuration/JwtSettings.cs
namespace SmartApp.WebApi.Configuration;
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; } = 10;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public int RefreshTokenRotationGraceSeconds { get; init; } = 30;
    public bool AllowInsecureHttp { get; init; } = false;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key))
            throw new InvalidOperationException("JWT Key is not configured.");
        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("JWT Issuer is not configured.");
        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("JWT Audience is not configured.");
    }
}
// Configuration/JwtSettings.cs
namespace SmartApp.WebApi.Configuration;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public bool AllowInsecureHttp { get; set; } = false;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Key)) throw new InvalidOperationException("JWT Key is not configured");
        if (string.IsNullOrEmpty(Issuer)) throw new InvalidOperationException("JWT Issuer is not configured");
        if (string.IsNullOrEmpty(Audience)) throw new InvalidOperationException("JWT Audience is not configured");
        if (Key.Length < 32) throw new InvalidOperationException("JWT Key must be at least 32 characters");
    }
}
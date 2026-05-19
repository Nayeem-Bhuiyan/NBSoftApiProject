namespace SmartApp.Application.DTOs.Auth;

public sealed class SessionInfoDto
{
    public string SessionId { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
    public string Platform { get; init; } = string.Empty;
    public string IpAddress { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsActive { get; init; }
}
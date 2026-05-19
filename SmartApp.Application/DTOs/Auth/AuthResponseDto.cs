namespace SmartApp.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    string SessionId,
    string TokenType,
    ApplicationUserDto User
)
{
    public AuthResponseDto() : this(
        string.Empty,
        string.Empty,
        default,
        default,
        string.Empty,
        "Bearer",
        null)
    { }
}
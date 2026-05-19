namespace SmartApp.Application.DTOs.Auth;

public sealed record RefreshTokenRequest(
    string RefreshToken,
    string DeviceId
);
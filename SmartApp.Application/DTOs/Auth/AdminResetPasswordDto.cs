namespace SmartApp.Application.DTOs.Auth;

public sealed record AdminResetPasswordDto(
    string UserId,
    string NewPassword
);
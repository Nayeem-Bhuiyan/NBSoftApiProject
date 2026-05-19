using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.ValueObjects;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Interfaces.Auth;

public interface IRefreshTokenService
{
    Task<Response<AuthResponseDto>> RefreshAsync(
        string refreshToken,
        DeviceFingerprint fingerprint,
        CancellationToken cancellationToken = default);

    Task<Response<bool>> RevokeSessionAsync(
        string sessionId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Response<bool>> RevokeAllUserSessionsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Response<List<SessionInfoDto>>> GetActiveSessionsAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.ValueObjects;
using SmartApp.Shared.Common;

namespace SmartApp.Infrastructure.Services.Auth;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IDistributedCache _cache;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public RefreshTokenService(
        IDistributedCache cache,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IConfiguration config)
    {
        _cache        = cache;
        _tokenService = tokenService;
        _userManager  = userManager;
        _mapper       = mapper;
        _config       = config;
    }

    public async Task<Response<AuthResponseDto>> RefreshAsync(
        string refreshToken,
        DeviceFingerprint fingerprint,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var tokenKey = $"refresh:token:{tokenHash}";

        var tokenJson = await _cache.GetStringAsync(tokenKey, cancellationToken);
        if (tokenJson is null)
            return Response<AuthResponseDto>.Failure("Refresh token is invalid or expired.");

        var tokenData = JsonSerializer.Deserialize<RefreshTokenData>(tokenJson)!;

        // ── Replay detection ──────────────────────────────────────────────
        if (tokenData.IsRevoked)
        {
            await HandleReplayAttackAsync(tokenData, cancellationToken);
            return Response<AuthResponseDto>.Failure("Refresh token has been revoked. Possible replay attack detected.");
        }

        // ── Device fingerprint validation ─────────────────────────────────
        if (tokenData.DeviceId != fingerprint.DeviceId)
            return Response<AuthResponseDto>.Failure("Device mismatch detected.");

        // ── Grace period check ────────────────────────────────────────────
        var graceSeconds = _config.GetValue<int>("Jwt:RefreshTokenRotationGraceSeconds", 30);
        if (DateTime.UtcNow > tokenData.ExpiresAt)
            return Response<AuthResponseDto>.Failure("Refresh token has expired.");

        // ── Resolve user ──────────────────────────────────────────────────
        var user = await _userManager.FindByIdAsync(tokenData.UserId);
        if (user is null)
            return Response<AuthResponseDto>.Failure("User not found.");

        // ── Revoke current token (rotate) ─────────────────────────────────
        await RevokeTokenAsync(tokenKey, tokenData, cancellationToken);

        // ── Issue new token pair ──────────────────────────────────────────
        var authResponse = await _tokenService.GenerateAuthResponseAsync(user, fingerprint, cancellationToken);

        var userDto = _mapper.Map<ApplicationUserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return Response<AuthResponseDto>.SuccessResponse(
            authResponse with { User = userDto },
            "Token refreshed successfully.");
    }

    public async Task<Response<bool>> RevokeSessionAsync(
        string sessionId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var userKey = $"refresh:user:{userId}:sessions";
        var sessionKey = $"refresh:session:{sessionId}";

        var existingJson = await _cache.GetStringAsync(userKey, cancellationToken);
        if (existingJson is null)
            return Response<bool>.Failure("No active sessions found.");

        var sessions = JsonSerializer.Deserialize<List<string>>(existingJson)!;
        if (!sessions.Contains(sessionId))
            return Response<bool>.Failure("Session not found.");

        // ── Remove session from user session list ─────────────────────────
        sessions.Remove(sessionId);
        await _cache.SetStringAsync(
            userKey,
            JsonSerializer.Serialize(sessions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(
                    _config.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7))
            },
            cancellationToken);

        // ── Remove session detail ─────────────────────────────────────────
        await _cache.RemoveAsync(sessionKey, cancellationToken);

        return Response<bool>.SuccessResponse(true, "Session revoked successfully.");
    }

    public async Task<Response<bool>> RevokeAllUserSessionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var userKey = $"refresh:user:{userId}:sessions";
        var existingJson = await _cache.GetStringAsync(userKey, cancellationToken);

        if (existingJson is not null)
        {
            var sessions = JsonSerializer.Deserialize<List<string>>(existingJson)!;

            foreach (var sessionId in sessions)
                await _cache.RemoveAsync($"refresh:session:{sessionId}", cancellationToken);
        }

        await _cache.RemoveAsync(userKey, cancellationToken);

        return Response<bool>.SuccessResponse(true, "All sessions revoked successfully.");
    }

    public async Task<Response<List<SessionInfoDto>>> GetActiveSessionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var userKey = $"refresh:user:{userId}:sessions";
        var existingJson = await _cache.GetStringAsync(userKey, cancellationToken);

        if (existingJson is null)
            return Response<List<SessionInfoDto>>.SuccessResponse(new List<SessionInfoDto>(), "No active sessions.");

        var sessionIds = JsonSerializer.Deserialize<List<string>>(existingJson)!;
        var sessions = new List<SessionInfoDto>();

        foreach (var sessionId in sessionIds)
        {
            var sessionJson = await _cache.GetStringAsync($"refresh:session:{sessionId}", cancellationToken);
            if (sessionJson is not null)
                sessions.Add(JsonSerializer.Deserialize<SessionInfoDto>(sessionJson)!);
        }

        return Response<List<SessionInfoDto>>.SuccessResponse(sessions, "Active sessions retrieved.");
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private async Task HandleReplayAttackAsync(RefreshTokenData tokenData, CancellationToken ct)
    {
        var familyKey = $"refresh:family:{tokenData.FamilyId}";
        var familyJson = await _cache.GetStringAsync(familyKey, ct);

        if (familyJson is null) return;

        var family = JsonSerializer.Deserialize<TokenFamilyData>(familyJson)!;

        // ← escalate: revoke entire family on replay
        var compromised = family with { IsCompromised = true, ReplayAttempts = family.ReplayAttempts + 1 };
        await _cache.SetStringAsync(familyKey, JsonSerializer.Serialize(compromised),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) }, ct);

        // ← revoke all user sessions on repeated replay
        if (compromised.ReplayAttempts >= 3)
            await RevokeAllUserSessionsAsync(tokenData.UserId, ct);
    }

    private async Task RevokeTokenAsync(string tokenKey, RefreshTokenData tokenData, CancellationToken ct)
    {
        var revoked = tokenData with { IsRevoked = true };
        var graceSeconds = _config.GetValue<int>("Jwt:RefreshTokenRotationGraceSeconds", 30);

        // ← keep revoked token in cache for grace period to detect replay
        await _cache.SetStringAsync(
            tokenKey,
            JsonSerializer.Serialize(revoked),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(graceSeconds)
            },
            ct);
    }

    private static string HashToken(string token)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
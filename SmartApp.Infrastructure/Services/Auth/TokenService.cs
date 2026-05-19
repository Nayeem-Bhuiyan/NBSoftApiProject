using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.ValueObjects;

namespace SmartApp.Infrastructure.Services.Auth;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDistributedCache _cache;

    public TokenService(
        IConfiguration config,
        UserManager<ApplicationUser> userManager,
        IDistributedCache cache)
    {
        _config      = config;
        _userManager = userManager;
        _cache       = cache;
    }

    public async Task<AuthResponseDto> GenerateAuthResponseAsync(
        ApplicationUser user,
        DeviceFingerprint fingerprint,
        CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = GenerateRefreshToken();
        var sessionId = Guid.NewGuid().ToString();
        var familyId = Guid.NewGuid().ToString();
        var tokenHash = HashToken(refreshToken);

        var accessExpiry = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 10));
        var refreshExpiry = DateTime.UtcNow.AddDays(
            _config.GetValue<int>("Jwt:RefreshTokenExpirationDays", 7));

        var tokenData = new RefreshTokenData(
            TokenHash: tokenHash,
            UserId: user.Id,
            SessionId: sessionId,
            FamilyId: familyId,
            DeviceId: fingerprint.DeviceId,
            UserAgent: fingerprint.UserAgent,
            IpAddress: fingerprint.IpAddress,
            Platform: fingerprint.Platform,
            CreatedAt: DateTime.UtcNow,
            ExpiresAt: refreshExpiry,
            IsRevoked: false,
            ReplacedBy: null
        );

        await StoreRefreshTokenAsync(tokenHash, tokenData, refreshExpiry, cancellationToken);
        await StoreFamilyAsync(familyId, sessionId, refreshExpiry, cancellationToken);
        await AddUserSessionAsync(user.Id, sessionId, tokenData, refreshExpiry, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken           = accessToken,
            RefreshToken          = refreshToken,
            AccessTokenExpiresAt  = accessExpiry,
            RefreshTokenExpiresAt = refreshExpiry,
            SessionId             = sessionId,
            TokenType             = "Bearer"
        };
    }

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,  user.Id),
            new(JwtRegisteredClaimNames.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier,    user.Id),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int>("Jwt:AccessTokenExpirationMinutes", 10));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private async Task StoreRefreshTokenAsync(
        string tokenHash,
        RefreshTokenData data,
        DateTime expiry,
        CancellationToken ct)
    {
        var ttl = expiry - DateTime.UtcNow;
        await _cache.SetStringAsync(
            $"refresh:token:{tokenHash}",
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
            ct);
    }

    private async Task StoreFamilyAsync(
        string familyId,
        string sessionId,
        DateTime expiry,
        CancellationToken ct)
    {
        var ttl = expiry - DateTime.UtcNow;
        var family = new TokenFamilyData(familyId, sessionId, DateTime.UtcNow, false, 0);
        await _cache.SetStringAsync(
            $"refresh:family:{familyId}",
            JsonSerializer.Serialize(family),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
            ct);
    }

    private async Task AddUserSessionAsync(
        string userId,
        string sessionId,
        RefreshTokenData tokenData,
        DateTime expiry,
        CancellationToken ct)
    {
        var sessionKey = $"refresh:session:{sessionId}";
        var userKey = $"refresh:user:{userId}:sessions";
        var ttl = expiry - DateTime.UtcNow;

        var sessionInfo = new SessionInfoDto
        {
            SessionId = sessionId,
            DeviceId  = tokenData.DeviceId,
            Platform  = tokenData.Platform,
            IpAddress = tokenData.IpAddress,
            UserAgent = tokenData.UserAgent,
            CreatedAt = tokenData.CreatedAt,
            ExpiresAt = tokenData.ExpiresAt,
            IsActive  = true
        };

        // store session detail
        await _cache.SetStringAsync(
            sessionKey,
            JsonSerializer.Serialize(sessionInfo),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
            ct);

        // add sessionId to user's session set
        var existingJson = await _cache.GetStringAsync(userKey, ct);
        var sessions = existingJson is not null
            ? JsonSerializer.Deserialize<List<string>>(existingJson)!
            : new List<string>();

        if (!sessions.Contains(sessionId))
            sessions.Add(sessionId);

        await _cache.SetStringAsync(
            userKey,
            JsonSerializer.Serialize(sessions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl },
            ct);
    }
}

// ── Internal Redis data models ────────────────────────────────────────────

internal sealed record RefreshTokenData(
    string TokenHash,
    string UserId,
    string SessionId,
    string FamilyId,
    string DeviceId,
    string UserAgent,
    string IpAddress,
    string Platform,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsRevoked,
    string? ReplacedBy
);

internal sealed record TokenFamilyData(
    string FamilyId,
    string SessionId,
    DateTime CreatedAt,
    bool IsCompromised,
    int ReplayAttempts
);
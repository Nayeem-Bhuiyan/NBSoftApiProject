using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.ValueObjects;
using SmartApp.Shared.Common;
using System.Security.Claims;

namespace SmartApp.WebApi.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMapper _mapper;

    public UserController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IMapper mapper)
    {
        _userManager         = userManager;
        _signInManager       = signInManager;
        _tokenService        = tokenService;
        _refreshTokenService = refreshTokenService;
        _mapper              = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(Response<string>.Failure("Invalid registration data."));

        var createObj = _mapper.Map<ApplicationUser>(model);
        createObj.isActive = true;

        var result = await _userManager.CreateAsync(createObj, model.Password);
        if (!result.Succeeded)
            return BadRequest(Response<string>.Failure(
                string.Join("; ", result.Errors.Select(e => e.Description))));

        var role = string.IsNullOrWhiteSpace(model.Role) ? "User" : model.Role;
        await _userManager.AddToRoleAsync(createObj, role);

        return Ok(Response<string>.SuccessResponse(null, "User registered successfully."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(Response<string>.Failure("Invalid login data."));

        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is null)
            return Unauthorized(Response<string>.Failure("Invalid username or password."));

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(Response<string>.Failure("Invalid username or password."));

        var fingerprint = BuildFingerprint(model.DeviceId);
        var authResponse = await _tokenService.GenerateAuthResponseAsync(user, fingerprint, ct);

        var userDto = _mapper.Map<ApplicationUserDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return Ok(Response<AuthResponseDto>.SuccessResponse(
            authResponse with { User = userDto }, "Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct = default)
    {
        var fingerprint = BuildFingerprint(request.DeviceId);
        var result = await _refreshTokenService.RefreshAsync(request.RefreshToken, fingerprint, ct);

        return result.isSuccess ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] string sessionId, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(Response<string>.Failure("Invalid token."));

        await _refreshTokenService.RevokeSessionAsync(sessionId, userId, ct);
        await _signInManager.SignOutAsync();

        return Ok(Response<string>.SuccessResponse(null, "Logout successful."));
    }

    [Authorize]
    [HttpPost("revoke-session/{sessionId}")]
    public async Task<IActionResult> RevokeSession(string sessionId, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(Response<string>.Failure("Invalid token."));

        var result = await _refreshTokenService.RevokeSessionAsync(sessionId, userId, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpPost("revoke-all-sessions")]
    public async Task<IActionResult> RevokeAllSessions(CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(Response<string>.Failure("Invalid token."));

        var result = await _refreshTokenService.RevokeAllUserSessionsAsync(userId, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetActiveSessions(CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Unauthorized(Response<string>.Failure("Invalid token."));

        var result = await _refreshTokenService.GetActiveSessionsAsync(userId, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private DeviceFingerprint BuildFingerprint(string deviceId) => new(
        DeviceId: deviceId,
        UserAgent: Request.Headers.UserAgent.ToString(),
        IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        Platform: Request.Headers["X-Platform"].FirstOrDefault() ?? "unknown"
    );
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.Features.Auth.UserManagement.Commands.AdminResetPassword;
using SmartApp.Application.Features.Auth.UserManagement.Commands.AssignUserRole;
using SmartApp.Application.Features.Auth.UserManagement.Commands.RevokeUserRole;
using SmartApp.Application.Features.Auth.UserManagement.Commands.SetUserActiveStatus;
using SmartApp.Application.Features.Auth.UserManagement.Queries.GetUserById;
using SmartApp.Application.Features.Auth.UserManagement.Queries.GetUsersPaged;
using SmartApp.WebApi.Logging;
using SmartApp.WebApi.RateLimit;

namespace SmartApp.WebApi.Controllers.Auth;

[Authorize(Policy = "DynamicPermission")]
[ApiController]
[Route("api/[controller]")]
public sealed class UserManagementController : ControllerBase
{
    private readonly ISender _sender;

    public UserManagementController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] string? searchTerm,
        [FromQuery] string? roleFilter,
        [FromQuery] bool? isActive,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(
            new GetUsersPagedQuery(searchTerm, roleFilter, isActive, pageIndex, pageSize, sortBy, sortDesc), ct);

        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(string userId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetUserByIdQuery(userId), ct);
        return result.isSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(
        [FromBody] AssignUserRoleCommand command,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("revoke-role")]
    public async Task<IActionResult> RevokeRole(
        [FromBody] RevokeUserRoleCommand command,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }
    [LogResponseBody]
    [HttpPatch("{userId}/active-status")]
    public async Task<IActionResult> SetActiveStatus(
        string userId,
        [FromBody] bool isActive,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new SetUserActiveStatusCommand(userId, isActive), ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }
    [LogResponseBody]
    [RateLimitPolicy("PasswordReset")]  // ← 3 req/hour per user
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] AdminResetPasswordCommand command,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }
}
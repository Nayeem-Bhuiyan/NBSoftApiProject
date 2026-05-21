using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartApp.Application.Features.Auth.RoleManagement.Commands.AssignPermission;
using SmartApp.Application.Features.Auth.RoleManagement.Commands.BulkAssignPermissions;
using SmartApp.Application.Features.Auth.RoleManagement.Commands.CreateRole;
using SmartApp.Application.Features.Auth.RoleManagement.Commands.DeleteRole;
using SmartApp.Application.Features.Auth.RoleManagement.Commands.UpdateRole;
using SmartApp.Application.Features.Auth.RoleManagement.Queries.GetAllRoles;
using SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRoleById;
using SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRolePermissions;

namespace SmartApp.WebApi.Controllers.Auth;

[Authorize(Policy = "DynamicPermission")]
[ApiController]
[Route("api/[controller]")]
public sealed class RoleController : ControllerBase
{
    private readonly ISender _sender;

    public RoleController(ISender sender)
    {
        _sender = sender;
    }

    // ── Role CRUD ─────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetAllRolesQuery(), ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetRoleByIdQuery(id), ct);
        return result.isSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command, CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command, CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct = default)
    {
        var result = await _sender.Send(new DeleteRoleCommand(id), ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    // ── Permission Assignment ─────────────────────────────────────────────

    [HttpGet("{roleId}/permissions")]
    public async Task<IActionResult> GetPermissions(string roleId, CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetRolePermissionsQuery(roleId), ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermission(
        [FromBody] AssignPermissionCommand command,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("bulk-assign-permissions")]
    public async Task<IActionResult> BulkAssignPermissions(
        [FromBody] BulkAssignPermissionsCommand command,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(command, ct);
        return result.isSuccess ? Ok(result) : BadRequest(result);
    }
}
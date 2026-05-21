using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.DeleteRole;

public sealed class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, Response<bool>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IPermissionService _permissionService;

    public DeleteRoleHandler(RoleManager<ApplicationRole> roleManager, IPermissionService permissionService)
    {
        _roleManager       = roleManager;
        _permissionService = permissionService;
    }

    public async Task<Response<bool>> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role is null)
            return Response<bool>.Failure("Role not found.");

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return Response<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        // ← invalidate permission cache on role delete
        await _permissionService.InvalidateRoleCacheAsync(request.Id);

        return Response<bool>.SuccessResponse(true, "Role deleted successfully.");
    }
}
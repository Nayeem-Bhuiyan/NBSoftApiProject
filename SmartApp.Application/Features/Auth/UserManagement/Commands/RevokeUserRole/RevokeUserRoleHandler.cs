using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.RevokeUserRole;

public sealed class RevokeUserRoleHandler : IRequestHandler<RevokeUserRoleCommand, Response<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IPermissionService _permissionService;

    public RevokeUserRoleHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IPermissionService permissionService)
    {
        _userManager       = userManager;
        _roleManager       = roleManager;
        _permissionService = permissionService;
    }

    public async Task<Response<bool>> Handle(RevokeUserRoleCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Response<bool>.Failure("User not found.");

        if (!await _userManager.IsInRoleAsync(user, request.RoleName))
            return Response<bool>.Failure($"User is not assigned to role '{request.RoleName}'.");

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return Response<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        // ← revoke all sessions — role change is a security-sensitive event
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role is not null)
            await _permissionService.InvalidateRoleCacheAsync(role.Id);

        return Response<bool>.SuccessResponse(true, $"Role '{request.RoleName}' revoked successfully.");
    }
}
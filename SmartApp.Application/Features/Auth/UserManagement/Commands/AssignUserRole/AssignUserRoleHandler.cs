using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.UserManagement.Commands.AssignUserRole;

public sealed class AssignUserRoleHandler : IRequestHandler<AssignUserRoleCommand, Response<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IPermissionService _permissionService;

    public AssignUserRoleHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IPermissionService permissionService)
    {
        _userManager       = userManager;
        _roleManager       = roleManager;
        _permissionService = permissionService;
    }

    public async Task<Response<bool>> Handle(AssignUserRoleCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Response<bool>.Failure("User not found.");

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
            return Response<bool>.Failure($"Role '{request.RoleName}' does not exist.");

        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            return Response<bool>.Failure($"User is already assigned to role '{request.RoleName}'.");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return Response<bool>.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        // ← invalidate permission cache — role change affects permissions
        var role = await _roleManager.FindByNameAsync(request.RoleName);
        if (role is not null)
            await _permissionService.InvalidateRoleCacheAsync(role.Id);

        return Response<bool>.SuccessResponse(true, $"Role '{request.RoleName}' assigned successfully.");
    }
}
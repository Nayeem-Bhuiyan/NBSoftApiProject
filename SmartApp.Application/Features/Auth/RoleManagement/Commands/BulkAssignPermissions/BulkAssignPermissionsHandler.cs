using MediatR;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.BulkAssignPermissions;

public sealed class BulkAssignPermissionsHandler : IRequestHandler<BulkAssignPermissionsCommand, Response<bool>>
{
    private readonly IRolePermissionRepository _repository;
    private readonly IPermissionService _permissionService;

    public BulkAssignPermissionsHandler(
        IRolePermissionRepository repository,
        IPermissionService permissionService)
    {
        _repository        = repository;
        _permissionService = permissionService;
    }

    public async Task<Response<bool>> Handle(BulkAssignPermissionsCommand request, CancellationToken ct)
    {
        var permissions = await _repository.GetPermissionsByControllerAsync(request.Controller, ct);
        if (!permissions.Any())
            return Response<bool>.Failure($"No permissions found for controller '{request.Controller}'.");

        var toAdd = new List<RolePermission>();
        var toUpdate = new List<RolePermission>();

        foreach (var permission in permissions)
        {
            var existing = await _repository.GetSingleAsync(request.RoleId, permission.Id, ct);

            if (existing is null)
                toAdd.Add(new RolePermission
                {
                    RoleId       = request.RoleId,
                    PermissionId = permission.Id,
                    IsGranted    = request.IsGranted
                });
            else
            {
                existing.IsGranted = request.IsGranted;
                toUpdate.Add(existing);
            }
        }

        if (toAdd.Any()) await _repository.AddRangeAsync(toAdd, ct);
        if (toUpdate.Any()) await _repository.UpdateRangeAsync(toUpdate, ct);

        await _repository.SaveChangesAsync(ct);

        // ← invalidate cache for affected role
        await _permissionService.InvalidateRoleCacheAsync(request.RoleId);

        return Response<bool>.SuccessResponse(true,
            $"Bulk {(request.IsGranted ? "grant" : "revoke")} applied to '{request.Controller}' successfully.");
    }
}
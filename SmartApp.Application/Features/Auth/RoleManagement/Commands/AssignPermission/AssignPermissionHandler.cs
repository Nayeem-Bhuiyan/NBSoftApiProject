using MediatR;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Commands.AssignPermission;

public sealed class AssignPermissionHandler : IRequestHandler<AssignPermissionCommand, Response<bool>>
{
    private readonly IRolePermissionRepository _repository;
    private readonly IPermissionService _permissionService;

    public AssignPermissionHandler(
        IRolePermissionRepository repository,
        IPermissionService permissionService)
    {
        _repository        = repository;
        _permissionService = permissionService;
    }

    public async Task<Response<bool>> Handle(AssignPermissionCommand request, CancellationToken ct)
    {
        var existing = await _repository.GetSingleAsync(request.RoleId, request.PermissionId, ct);

        if (existing is null)
        {
            await _repository.AddAsync(new RolePermission
            {
                RoleId       = request.RoleId,
                PermissionId = request.PermissionId,
                IsGranted    = request.IsGranted
            }, ct);
        }
        else
        {
            existing.IsGranted = request.IsGranted;
            await _repository.UpdateAsync(existing, ct);
        }

        await _repository.SaveChangesAsync(ct);

        // ← invalidate cache so next request re-reads from DB
        await _permissionService.InvalidateRoleCacheAsync(request.RoleId);

        return Response<bool>.SuccessResponse(true,
            request.IsGranted ? "Permission granted." : "Permission revoked.");
    }
}
using MediatR;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Shared.Common;

namespace SmartApp.Application.Features.Auth.RoleManagement.Queries.GetRolePermissions;

public sealed class GetRolePermissionsHandler : IRequestHandler<GetRolePermissionsQuery, Response<List<RolePermissionDto>>>
{
    private readonly IRolePermissionRepository _repository;

    public GetRolePermissionsHandler(IRolePermissionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Response<List<RolePermissionDto>>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        var permissions = await _repository.GetByRoleIdAsync(request.RoleId, ct);

        return Response<List<RolePermissionDto>>.SuccessResponse(
            permissions, "Role permissions retrieved successfully.");
    }
}
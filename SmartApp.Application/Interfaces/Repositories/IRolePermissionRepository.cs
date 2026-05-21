using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;

namespace SmartApp.Application.Interfaces.Repositories;

public interface IRolePermissionRepository
{
    Task<List<RolePermissionDto>> GetByRoleIdAsync(string roleId, CancellationToken ct = default);
    Task<RolePermission?> GetSingleAsync(string roleId, int permissionId, CancellationToken ct = default);
    Task<List<Permission>> GetPermissionsByControllerAsync(string controller, CancellationToken ct = default);
    Task AddAsync(RolePermission rolePermission, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct = default);
    Task UpdateAsync(RolePermission rolePermission, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
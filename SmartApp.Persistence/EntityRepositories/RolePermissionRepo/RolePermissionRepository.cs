using Microsoft.EntityFrameworkCore;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Persistence.DBContext;

namespace SmartApp.Persistence.EntityRepositories.PermissionRepo;

public sealed class RolePermissionRepository : IRolePermissionRepository
{
    private readonly ApplicationDbContext _db;

    public RolePermissionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<RolePermissionDto>> GetByRoleIdAsync(string roleId, CancellationToken ct = default)
    {
        return await _db.Permissions
            .AsNoTracking()
            .Select(p => new RolePermissionDto
            {
                PermissionId = p.Id,
                Controller   = p.Controller,
                Action       = p.Action,
                HttpMethod   = p.HttpMethod,
                DisplayName  = p.DisplayName,
                IsGranted    = _db.RolePermissions
                    .Any(rp => rp.RoleId == roleId &&
                               rp.PermissionId == p.Id &&
                               rp.IsGranted)
            })
            .ToListAsync(ct);
    }

    public async Task<RolePermission?> GetSingleAsync(string roleId, int permissionId, CancellationToken ct = default)
    {
        return await _db.RolePermissions
            .FirstOrDefaultAsync(rp =>
                rp.RoleId == roleId &&
                rp.PermissionId == permissionId, ct);
    }

    public async Task<List<Permission>> GetPermissionsByControllerAsync(string controller, CancellationToken ct = default)
    {
        return await _db.Permissions
            .AsNoTracking()
            .Where(p => p.Controller.ToLower() == controller.ToLower())
            .ToListAsync(ct);
    }

    public async Task AddAsync(RolePermission rolePermission, CancellationToken ct = default)
    {
        await _db.RolePermissions.AddAsync(rolePermission, ct);
    }

    public async Task AddRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct = default)
    {
        await _db.RolePermissions.AddRangeAsync(rolePermissions, ct);
    }

    public Task UpdateAsync(RolePermission rolePermission, CancellationToken ct = default)
    {
        _db.RolePermissions.Update(rolePermission);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<RolePermission> rolePermissions, CancellationToken ct = default)
    {
        _db.RolePermissions.UpdateRange(rolePermissions);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
using Microsoft.EntityFrameworkCore;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.Repositories; // ← FIX: was IAuth, should be IRepositories
using SmartApp.Domain.Entities.Auth;
using SmartApp.Persistence.DBContext;

namespace SmartApp.Persistence.EntityRepositories.PermissionRepo;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _db;

    public PermissionRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> PermissionExistsAsync(string controller, string action, string httpMethod)
    {
        return await _db.Permissions.AnyAsync(p =>
            p.Controller == controller &&
            p.Action     == action &&
            p.HttpMethod == httpMethod);
    }

    public async Task AddPermissionAsync(Permission permission) 
    {
        await _db.Permissions.AddAsync(permission);
    }

    public async Task<List<PermissionCacheEntry>> GetGrantedPermissionsAsync(string roleId)
    {
        return await _db.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId && rp.IsGranted)
            .Select(rp => new PermissionCacheEntry(
                rp.Permission.Controller,
                rp.Permission.Action,
                rp.Permission.HttpMethod))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }

}
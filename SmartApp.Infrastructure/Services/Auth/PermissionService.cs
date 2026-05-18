using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SmartApp.Application.DTOs.Auth;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Domain.Entities.Auth;

namespace SmartApp.Infrastructure.Services.Auth;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    public PermissionService(IPermissionRepository permissionRepository, IDistributedCache cache)
    {
        _permissionRepository = permissionRepository;
        _cache                = cache;
    }

    public async Task<bool> HasPermissionAsync(string roleId, string controller, string action, string httpMethod)
    {
        var granted = await GetGrantedPermissionsFromCacheAsync(roleId);

        return granted.Any(p =>
            p.Controller.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
            p.Action.Equals(action, StringComparison.OrdinalIgnoreCase) &&
            p.HttpMethod.Equals(httpMethod, StringComparison.OrdinalIgnoreCase));
    }

    public async Task SeedPermissionsAsync(IEnumerable<PermissionDefinition> discovered)
    {
        foreach (var def in discovered)
        {
            var exists = await _permissionRepository.PermissionExistsAsync(
                def.Controller, def.Action, def.HttpMethod);

            if (!exists)
            {
                await _permissionRepository.AddPermissionAsync(new Permission
                {
                    Controller  = def.Controller,
                    Action      = def.Action,
                    HttpMethod  = def.HttpMethod,
                    DisplayName = def.DisplayName
                });
            }
        }

        await _permissionRepository.SaveChangesAsync();
    }

    public async Task InvalidateRoleCacheAsync(string roleId)
    {
        await _cache.RemoveAsync(BuildCacheKey(roleId));
    }

    private async Task<List<PermissionCacheEntry>> GetGrantedPermissionsFromCacheAsync(string roleId)
    {
        var cacheKey = BuildCacheKey(roleId);
        var cached = await _cache.GetStringAsync(cacheKey);

        if (cached is not null)
            return JsonSerializer.Deserialize<List<PermissionCacheEntry>>(cached)!;

        var permissions = await _permissionRepository.GetGrantedPermissionsAsync(roleId);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(permissions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheTtl
            });

        return permissions;
    }

    private static string BuildCacheKey(string roleId) => $"permissions:role:{roleId}";
}
using SmartApp.Application.DTOs.Auth;
using SmartApp.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartApp.Application.Interfaces.Auth
{
    public interface IPermissionRepository
    {
        Task<bool> PermissionExistsAsync(string controller, string action, string httpMethod);
        Task AddPermissionAsync(Permission permission);
        Task<List<PermissionCacheEntry>> GetGrantedPermissionsAsync(string roleId);
        Task SaveChangesAsync();
    }
}



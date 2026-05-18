
namespace SmartApp.Application.Interfaces.Auth;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(string roleId, string controller, string action, string httpMethod);
    Task SeedPermissionsAsync(IEnumerable<PermissionDefinition> discovered);
    Task InvalidateRoleCacheAsync(string roleId);
}

public record PermissionDefinition(string Controller, string Action, string HttpMethod, string DisplayName);
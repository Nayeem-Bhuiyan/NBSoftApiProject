using SmartApp.Application.Interfaces.Auth;

namespace SmartApp.Infrastructure.Seeds;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        IPermissionService permissionService,
        IEnumerable<PermissionDefinition> discovered) // ← pre-discovered, passed in
    {
        await permissionService.SeedPermissionsAsync(discovered);
    }
}
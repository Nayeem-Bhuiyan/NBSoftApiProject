// Extensions/MigrationExtensions.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Persistence.DBContext;
using SmartApp.WebApi.Authorization;
using SmartApp.WebApi.Data;   // Adjust if your DbContext namespace is different

namespace SmartApp.WebApi.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        // ← REMOVE: seedScope is redundant — reuse the same scope above
        // using var seedScope = app.Services.CreateScope(); 

        try
        {
            await context.Database.MigrateAsync();

            await RoleSeeder.SeedAsync(roleManager);

            // ← PermissionDiscoveryService stays in WebApi — correct
            var discovered = PermissionDiscoveryService.Discover(typeof(Program).Assembly);
            await permissionService.SeedPermissionsAsync(discovered);

            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations");
            throw;
        }
    }
}

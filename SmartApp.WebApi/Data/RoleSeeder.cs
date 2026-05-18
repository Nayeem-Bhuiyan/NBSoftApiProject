using Microsoft.AspNetCore.Identity;
using SmartApp.Domain.Entities.Auth;

namespace SmartApp.WebApi.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
    {
        string[] roles = ["Admin", "Manager", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }
    }
}

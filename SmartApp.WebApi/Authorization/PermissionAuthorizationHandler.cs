using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Domain.Entities.Auth;
using System.Security.Claims;

namespace SmartApp.WebApi.Authorization;

public class PermissionRequirement : IAuthorizationRequirement { }

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _permissionService = permissionService;
        _userManager       = userManager;
        _roleManager       = roleManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            context.Fail();
            return;
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        var routeData = httpContext.GetRouteData();
        var controller = routeData.Values["controller"]?.ToString() ?? string.Empty;
        var action = routeData.Values["action"]?.ToString()     ?? string.Empty;
        var httpMethod = httpContext.Request.Method;

        //temp code check role
        // ← ADD temporary debug
        //Console.WriteLine($">>> UserId: {userId}");
        //Console.WriteLine($">>> Roles: {string.Join(", ", roleNames)}");
        //Console.WriteLine($">>> Controller: {controller}");
        //Console.WriteLine($">>> Action: {action}");
        //Console.WriteLine($">>> HttpMethod: {httpMethod}");


        foreach (var roleName in roleNames)
        {
            var roleEntity = await _roleManager.FindByNameAsync(roleName);
            if (roleEntity is null) continue;

            //Console.WriteLine($">>> Checking RoleId: {roleEntity.Id} for permission...");

            var hasPermission = await _permissionService.HasPermissionAsync(roleEntity.Id, controller, action, httpMethod);

            //Console.WriteLine($">>> HasPermission: {hasPermission}");


            if (hasPermission)
            {
                context.Succeed(requirement);
                return;
            }
        }

        context.Fail();
    }
}
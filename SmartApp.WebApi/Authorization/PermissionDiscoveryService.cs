using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SmartApp.Application.Interfaces.Auth;

namespace SmartApp.WebApi.Authorization;

public static class PermissionDiscoveryService
{
    public static IEnumerable<PermissionDefinition> Discover(Assembly assembly)
    {
        var controllerTypes = assembly.GetTypes()
            .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var controller in controllerTypes)
        {
            var controllerName = controller.Name.Replace("Controller", string.Empty);

            var actions = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.GetCustomAttributes<HttpMethodAttribute>().Any());

            foreach (var action in actions)
            {
                var httpAttrs = action.GetCustomAttributes<HttpMethodAttribute>();

                foreach (var httpAttr in httpAttrs)
                {
                    var httpMethod = httpAttr switch
                    {
                        HttpGetAttribute => "GET",
                        HttpPostAttribute => "POST",
                        HttpPutAttribute => "PUT",
                        HttpDeleteAttribute => "DELETE",
                        HttpPatchAttribute => "PATCH",
                        _ => "GET"
                    };

                    yield return new PermissionDefinition(
                        Controller: controllerName,
                        Action: action.Name,
                        HttpMethod: httpMethod,
                        DisplayName: $"{controllerName} - {action.Name}");
                }
            }
        }
    }
}
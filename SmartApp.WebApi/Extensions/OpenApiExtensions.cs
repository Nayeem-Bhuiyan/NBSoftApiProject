// Extensions/OpenApiExtensions.cs
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;


namespace SmartApp.WebApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            // Use multiple transformers for better separation of concerns
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddDocumentTransformer<ApiInfoDocumentTransformer>();
            options.AddOperationTransformer<AddSecurityRequirementOperationTransformer>();
        });

        return services;
    }
}

// Separate transformer for security scheme
internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type        = SecuritySchemeType.Http,
            Scheme      = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token."
        });

        return Task.CompletedTask;
    }
}

// Separate transformer for API info
internal sealed class ApiInfoDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "SmartApp API",
            Version = "v1",
            Description = "SmartApp REST API with JWT Authentication",
            Contact = new OpenApiContact
            {
                Name = "SmartApp Support",
                Email = "support@smartapp.com"
            }
        };

        return Task.CompletedTask;
    }
}

// Separate transformer for security requirements
internal sealed class AddSecurityRequirementOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var hasAuthorizeAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .Any(em => em is Microsoft.AspNetCore.Authorization.AuthorizeAttribute);

        if (hasAuthorizeAttribute)
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer"),
                    new List<string>()
                }
            });
        }

        return Task.CompletedTask;
    }
}
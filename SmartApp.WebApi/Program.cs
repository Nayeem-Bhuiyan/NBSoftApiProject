using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using SmartApp.Application;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.ModelMapper;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Infrastructure;
using SmartApp.Infrastructure.Services.Auth;
using SmartApp.Persistence;
using SmartApp.Persistence.DBContext;
using SmartApp.WebApi.Authorization;
using SmartApp.WebApi.Configuration;
using SmartApp.WebApi.Extensions;
using SmartApp.WebApi.Logging;
using SmartApp.WebApi.Middleware;
using SmartApp.WebApi.RateLimit;

var builder = WebApplication.CreateBuilder(args);

#region SerioLog_Settings
builder.AddSerilogLogging();
builder.Services.Configure<LogSettings>(
builder.Configuration.GetSection(LogSettings.SectionName));
#endregion

var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>();
jwtSettings?.Validate();

builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<RateLimitFilter>();
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit            = false;
    options.Password.RequiredLength          = 4;
    options.Password.RequireNonAlphanumeric  = false;
    options.Password.RequireUppercase        = false;
    options.Password.RequireLowercase        = false;
    options.User.RequireUniqueEmail          = true;
    options.Lockout.DefaultLockoutTimeSpan   = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts  = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DynamicPermission", policy =>
        policy.AddRequirements(new PermissionRequirement()));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection string not configured.");
});

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
        ?? new[] { "http://localhost:4200", "http://localhost:5232" };

    options.AddPolicy("SmartAppCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddOpenApiWithAuth(builder.Configuration);
builder.Services.AddApplicationDI();
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI();
builder.Services.AddAutoMapper(config => config.AddProfile<MappingConfig>());

builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("AppDbConnection")!,
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(5))
    .AddRedis(
        builder.Configuration.GetConnectionString("Redis")!,
        name: "redis",
        timeout: TimeSpan.FromSeconds(3));

var app = builder.Build();
await app.ApplyMigrationsAsync();

app.UseCors("SmartAppCors");
app.UseHttpsRedirection();

var apiDocConfig = builder.Configuration.GetSection("ApiDocumentation");
var enableOpenApi = apiDocConfig.GetValue<bool>("EnableOpenApi");
var enableScalar = apiDocConfig.GetValue<bool>("EnableScalar");

if (enableOpenApi)
{
    app.MapOpenApi();
}

if (enableScalar)
{
    app.MapScalarApiReference(options =>
    {
        options.Title = "SmartApp API";
        options.Theme = ScalarTheme.Purple;
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

app.UseMiddleware<CorrelationIdMiddleware>(); 
app.UseMiddleware<RequestLoggingMiddleware>(); 
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
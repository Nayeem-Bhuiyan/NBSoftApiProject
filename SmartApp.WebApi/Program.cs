// Program.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.ModelMapper;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Infrastructure;
using SmartApp.Infrastructure.Services.Auth;
using SmartApp.Persistence;
using SmartApp.Persistence.DBContext;
using SmartApp.WebApi.Authorization;
using SmartApp.WebApi.Configuration;
using SmartApp.WebApi.Data;
using SmartApp.WebApi.Extensions;
using SmartApp.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// 1. STRONGLY-TYPED SETTINGS VALIDATION
// ─────────────────────────────────────────────
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    .Get<JwtSettings>();
jwtSettings?.Validate();

// ─────────────────────────────────────────────
// 2. CORE MVC
// ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ─────────────────────────────────────────────
// 3. IDENTITY — must be before AddAuthentication
// ─────────────────────────────────────────────
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

// ─────────────────────────────────────────────
// 4. JWT AUTHENTICATION
// ─────────────────────────────────────────────
builder.Services.AddJwtAuthentication(builder.Configuration);

// ─────────────────────────────────────────────
// 5. PERMISSION SERVICES
// ─────────────────────────────────────────────
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// ─────────────────────────────────────────────
// 6. AUTHORIZATION
// ─────────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DynamicPermission", policy =>
        policy.AddRequirements(new PermissionRequirement()));
});

// ─────────────────────────────────────────────
// 7. REDIS DISTRIBUTED CACHE
// ─────────────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Redis connection string not configured.");
});

// ─────────────────────────────────────────────
// 8. CORS
// ─────────────────────────────────────────────
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

// ─────────────────────────────────────────────
// 9. OPENAPI / SCALAR
// ─────────────────────────────────────────────
builder.Services.AddOpenApiWithAuth(builder.Configuration);

// ─────────────────────────────────────────────
// 10. APPLICATION SERVICES
// ─────────────────────────────────────────────
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI();
builder.Services.AddAutoMapper(config => config.AddProfile<MappingConfig>());

// ─────────────────────────────────────────────
// BUILD
// ─────────────────────────────────────────────
var app = builder.Build();

// ─────────────────────────────────────────────
// 11. DATABASE MIGRATIONS + SEEDING
// ─────────────────────────────────────────────
await app.ApplyMigrationsAsync();


// ─────────────────────────────────────────────
// 12. MIDDLEWARE PIPELINE — ORDER IS CRITICAL
// ─────────────────────────────────────────────
app.UseCors("SmartAppCors");            // 1. CORS first — handles preflight OPTIONS
app.UseHttpsRedirection();              // 2. HTTPS redirect

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "SmartApp API";
        options.Theme = ScalarTheme.Purple;
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>(); // 3. Global error handler
app.UseAuthentication();                        // 4. Validate JWT
app.UseAuthorization();                         // 5. Evaluate policies
app.MapControllers();

app.Run();
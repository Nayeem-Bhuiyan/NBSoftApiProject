// Program.cs - Clean and maintainable
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SmartApp.Application.ModelMapper;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Infrastructure;
using SmartApp.Persistence;
using SmartApp.Persistence.DBContext;
using SmartApp.WebApi.Configuration;
using SmartApp.WebApi.Extensions;
using SmartApp.WebApi.Middleware;


var builder = WebApplication.CreateBuilder(args);

// 1. Configure strongly-typed settings
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
jwtSettings?.Validate();

// 2. Add services with proper separation
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
     .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Add authentication and OpenAPI
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddOpenApiWithAuth(builder.Configuration);

// 4. Add authorization policies (optional but recommended)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireVerifiedEmail", policy => policy.RequireClaim("email_verified", "true"));
});

// 5. Add CORS with specific origins (not "*" in production)
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://127.0.0.1:5232", "http://localhost:5232" };

    options.AddPolicy("SmartAppCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 6. Add other services...
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI();
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingConfig>();
});
var app = builder.Build();

// Configure pipeline
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

app.UseHttpsRedirection();
app.UseCors("SmartAppCors"); // Use named policy instead of "AllowAll"
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
await app.ApplyMigrationsAsync();
app.Run();

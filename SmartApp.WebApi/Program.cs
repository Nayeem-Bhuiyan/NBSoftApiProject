using Microsoft.AspNetCore.Identity;
using SmartApp.Application.DTOs.Common;
using SmartApp.Application.ModelMapper;
using SmartApp.Infrastructure;
using SmartApp.Persistence;

var options = new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
};

var builder = WebApplication.CreateBuilder(options);

builder.Logging.AddJsonConsole();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingConfig));

// Register custom services
builder.Services.AddPersistenceDI(builder.Configuration);
builder.Services.AddInfrastructureDI();

//builder.Services.AddHttpContextAccessor();

builder.Services.Configure<FileImageSettings>(builder.Configuration.GetSection("FileImageSettings"));

// Identity options config
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts = 5;
});

#region Swagger_Config_1
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SmartApp API",
        Version = "v1",
        Description = "API documentation for SmartApp Web API",
        Contact = new()
        {
            Name = "Nayeem Bhuiyan",
            Email = "nayeem.datasoft2022@gmail.com",
        }
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    #region Swagger_Config_2
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartApp API V1");
        options.RoutePrefix = "swagger";
    });
    #endregion
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

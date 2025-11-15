using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartApp.Application.DTOs.Common;
using SmartApp.Application.ModelMapper;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Infrastructure;
using SmartApp.Persistence;
using SmartApp.Persistence.DBContext;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using AutoMapper;
      // ✅ For OpenApi types



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


builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingConfig>());

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

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

#region Jwt_config
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

#endregion

#region Swagger_Config_1

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartApp API",
        Version = "v1",
        Description = "API documentation"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    //c.OperationFilter<SecurityRequirementsOperationFilter>();
});


#endregion


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartApp API V1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<SmartApp.WebApi.Middleware.GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


#region Helper_Filter
//public class SecurityRequirementsOperationFilter : IOperationFilter
//{
//    public void Apply(OpenApiOperation operation, OperationFilterContext context)
//    {
//        if (operation.Security == null)
//            operation.Security = new List<OpenApiSecurityRequirement>();

//        var securityRequirement = new OpenApiSecurityRequirement
//        {
//            {
//                new OpenApiSecurityScheme
//                {
//                    Reference = new OpenApiReference
//                    {
//                        Type = ReferenceType.SecurityScheme,
//                        Id = "Bearer"
//                    },
//                    Scheme = "Bearer",
//                    Name = "Authorization",
//                    In = ParameterLocation.Header
//                },
//                new List<string>()
//            }
//        };

//        operation.Security.Add(securityRequirement);
//    }
//}
#endregion

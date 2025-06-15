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

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Add a distributed memory cache


#region AddSession
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".SmartEducation.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#endregion

#region IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    //options.User.AllowedUserNameCharacters = true;
    //Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts = 5;

});

#endregion


#region Package_Services_Activation
builder.Services.AddAutoMapper(typeof(MappingConfig));

#endregion

#region Config_CustomServices_Activation
//builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddDataAccessLayerServicesActivation(builder.Configuration);
builder.Services.AddServiceLayerServicesActivation();
#endregion


#region Custom_Service_Register
// Bind appsettings section to POCO
builder.Services.Configure<FileImageSettings>(builder.Configuration.GetSection("FileImageSettings"));

#endregion


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    // Set metadata
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

    // JWT Bearer auth (if needed)
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
            new string[] {}
        }
    });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartApp API V1");
        options.RoutePrefix = "swagger"; // default
    });
}

// OR: if you want Swagger always enabled
//app.UseSwagger();
//app.UseSwaggerUI();

app.UseDefaultFiles(); // Must be before UseStaticFiles
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();

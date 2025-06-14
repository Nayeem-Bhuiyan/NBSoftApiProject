using Microsoft.AspNetCore.Identity;
using SmartApp.Application.DTOs.Common;
using SmartApp.Application.ModelMapper;
using SmartApp.Infrastructure;
using SmartApp.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

builder.Logging.AddJsonConsole();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Add a distributed memory cache
//builder.Services.AddRazorPages().AddNewtonsoftJson();

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



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();

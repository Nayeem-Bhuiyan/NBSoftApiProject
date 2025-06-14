using SmartApp.Application.DTOs.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();



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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.Extensions.DependencyInjection;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.MasterData;
using SmartApp.Infrastructure.Services.Auth;
using SmartApp.Infrastructure.Services.MasterData;


namespace SmartApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}

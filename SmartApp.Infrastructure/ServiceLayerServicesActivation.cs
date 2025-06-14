using Microsoft.Extensions.DependencyInjection;
using SmartApp.Application.Interfaces.MasterData;
using SmartApp.Infrastructure.Services.MasterData;


namespace SmartApp.Infrastructure
{
   public static class ServiceLayerServicesActivation
    {
        public static void AddServiceLayerServicesActivation(this IServiceCollection services)
        {
            services.AddScoped<ICountryService,CountryService>();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Persistence.DBContext;
using SmartApp.Persistence.Repositories;


namespace SmartApp.Persistence
{
    public static class DependencyInjection
    {
        public static void AddPersistenceDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(
                    configuration.GetConnectionString("AppDbConnection"),
                    x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                )
            );
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}




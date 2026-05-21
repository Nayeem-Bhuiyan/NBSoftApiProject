using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApp.Application.Interfaces.Auth;
using SmartApp.Application.Interfaces.Repositories;
using SmartApp.Persistence.DBContext;
using SmartApp.Persistence.EntityRepositories.PermissionRepo;
using SmartApp.Persistence.EntityRepositories.User;
using SmartApp.Persistence.Repositories;

namespace SmartApp.Persistence
{
    public static class PersistenceDI
    {
        public static void AddPersistenceDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("AppDbConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );
                    }
                )
            );
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
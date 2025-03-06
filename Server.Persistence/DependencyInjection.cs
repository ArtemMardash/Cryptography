using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Server.Application.Interfaces;

namespace Server.Persistence;

public static class DependencyInjection
{
    public static void RegisterPersistence(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPackageRepository, PackageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<ServerContext>(opt =>
        {
            opt.UseNpgsql(connectionString,
                builder => builder.MigrationsAssembly(typeof(ServerContext).Assembly.GetName().Name));
        });
    }
}
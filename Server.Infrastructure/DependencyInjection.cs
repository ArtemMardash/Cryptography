using Microsoft.Extensions.DependencyInjection;
using Server.Application.Interfaces;
using Server.Infrastructure.Services;

namespace Server.Infrastructure;

public static class DependencyInjection
{
    
    public static void RegisterInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IZipService, ZipService>();
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining(typeof(Server.Application.DependencyInjection)));
    }
}
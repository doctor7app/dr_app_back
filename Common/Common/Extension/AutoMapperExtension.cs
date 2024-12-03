using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Extension;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperConfigurationV2(this IServiceCollection services, Assembly assembly)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));
        return services;
    }

    public static IServiceCollection AddAutoMapperConfigurationV2(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(cfg =>
        {
            foreach (var assembly in assemblies)
            {
                cfg.AddMaps(assembly);
            }
        });

        return services;
    }
}
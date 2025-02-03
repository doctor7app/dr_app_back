using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Extension.Services;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services, params Assembly[] assemblies)
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
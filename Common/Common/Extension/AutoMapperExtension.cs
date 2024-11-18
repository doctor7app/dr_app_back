using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Extension;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
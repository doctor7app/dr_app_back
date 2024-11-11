using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Common.Extension;

public static class AutoMapperExtension
{
    public static IServiceCollection AutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}
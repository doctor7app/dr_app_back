using Microsoft.Extensions.DependencyInjection;

namespace Common.Extension;

public static class AutoMapperExtension
{
    public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
}
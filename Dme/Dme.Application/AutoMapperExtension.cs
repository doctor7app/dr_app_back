using Microsoft.Extensions.DependencyInjection;

namespace Dme.Application
{
    public static class AutoMapperExtension
    {
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }
    }
}
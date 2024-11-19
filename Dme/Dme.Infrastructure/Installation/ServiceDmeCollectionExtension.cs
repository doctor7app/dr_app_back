using Common.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace Dme.Infrastructure.Installation
{
    public static class ServiceDmeCollectionExtension
    {
        /// <summary>
        /// Domain Centric Services related to the application
        /// Custom Services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDmeServiceCollection(this IServiceCollection services)
        {
            services.AddAutoMapperConfiguration();
            return services;
        }
    }
}
using System.Reflection;
using Common.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            services.AutoMapper();
            var interfaces = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsInterface).ToList();
            foreach (var item in interfaces)
            {
                var implementation = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(p => p.IsClass && (item.IsAssignableFrom(p) || p.ImplementsGenericInterface(item)));
                if (implementation != null)
                {
                    services.TryAdd(new ServiceDescriptor(item, implementation, ServiceLifetime.Transient));
                }
            }

            return services;
        }
        
    }
}
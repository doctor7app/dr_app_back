using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Patients.Dtos.Classes.Patient;
using Patients.Dtos.Installation;
using Patients.Services.Implementation;
using Patients.Services.Interfaces;
using AutoMapper.Internal;
using Common.Extension;

namespace Patients.Services.Installation
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Domain Centric Services related to the application
        /// Custom Services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPatientServiceCollection(this IServiceCollection services)
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
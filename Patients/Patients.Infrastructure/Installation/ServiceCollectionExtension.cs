using Common.Extension;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patients.Application.Interfaces;
using Patients.Infrastructure.Implementation;

namespace Patients.Infrastructure.Installation
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
            var applicationAssembly = typeof(Application.MappingProfile).Assembly;
            var infrastructureAssembly = typeof(MessageMappingProfile).Assembly;
            services.AddAutoMapperConfigurationV2(applicationAssembly, infrastructureAssembly);

            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IAdresseService, AdresseService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IMedicalInfoService, MedicalInfoService>();
            return services;
        }

        /// <summary>
        /// Add Configuration for mass transit
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPatientMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(a =>
            {
                a.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("patient", false));
                a.UsingRabbitMq((context, config) =>
                {
                    var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                    var rabbitMqPort = configuration["RabbitMQ:Port"] ?? "5672";
                    var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
                    var rabbitMqPass = configuration["RabbitMQ:Password"] ?? "guest";
                    var rabbitMqUri = $"amqp://{rabbitMqUser}:{rabbitMqPass}@{rabbitMqHost}:{rabbitMqPort}";
                    config.Host(new Uri(rabbitMqUri));
                    config.ConfigureEndpoints(context);
                    config.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                    });
                });
            });

            return services;

        }
    }
}
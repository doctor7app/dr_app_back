using Common.Extension;
using Dme.Infrastructure.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
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

        /// <summary>
        /// Add Configuration for mass transit
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDmeMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(a =>
            {
                a.AddConsumersFromNamespaceContaining<PatientCreatedConsumer>();

                a.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("dme",false));

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
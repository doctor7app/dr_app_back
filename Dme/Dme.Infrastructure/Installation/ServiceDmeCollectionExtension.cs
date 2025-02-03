using Common.Extension.Services;
using Dme.Application.Interfaces;
using Dme.Infrastructure.Consumers;
using Dme.Infrastructure.Implementation;
using Dme.Infrastructure.Persistence;
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
            
            var applicationAssembly = typeof(Application.MappingProfile).Assembly;
            var infrastructureAssembly = typeof(MessageMappingProfile).Assembly;
            services.AddAutoMapperConfiguration(applicationAssembly, infrastructureAssembly);
            services.AddTransient<IConsultationService, ConsultationService>();
            services.AddTransient<IDiagnosticService, DiagnosticService>();
            services.AddTransient<IDmeService, DmeService>();
            services.AddTransient<ITreatmentService, TreatmentService>();
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

                a.AddEntityFrameworkOutbox<DmeDbContext>(opt =>
                {
                    opt.QueryDelay = TimeSpan.FromSeconds(100);
                    opt.UsePostgres();
                    opt.UseBusOutbox();
                });
                a.UsingRabbitMq((context, config) =>
                {
                    var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                    var rabbitMqPort = configuration["RabbitMQ:Port"] ?? "5672";
                    var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
                    var rabbitMqPass = configuration["RabbitMQ:Password"] ?? "guest";
                    var rabbitMqUri = $"amqp://{rabbitMqUser}:{rabbitMqPass}@{rabbitMqHost}:{rabbitMqPort}";
                    config.Host(new Uri(rabbitMqUri));
                    config.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Incremental(5, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));
                    });
                    config.ConfigureEndpoints(context);
                });
            });
            return services;
        }
    }
}
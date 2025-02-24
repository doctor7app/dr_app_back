using Common.Extension.Services;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Interfaces;
using Prescriptions.Application.Validators;
using Prescriptions.Infrastructure.Implementation;
using Prescriptions.Infrastructure.Persistence;
using Prescriptions.Infrastructure.Services;

namespace Prescriptions.Infrastructure.Installation
{
    public static class ServicePrescriptionCollectionExtension
    {
        /// <summary>
        /// Domain Centric Services related to the application
        /// Custom Services.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPrescriptionServiceCollection(this IServiceCollection services)
        {
            var applicationAssembly = typeof(Application.MappingProfile).Assembly;
            var infrastructureAssembly = typeof(MessageMappingProfile).Assembly;
            services.AddAutoMapperConfiguration(applicationAssembly, infrastructureAssembly);

            services.AddTransient<IPrescriptionService, PrescriptionService>();
            services.AddTransient<IPrescriptionItemService, PrescriptionItemService>();
            services.AddTransient<IEventStoreService, EventStoreService>();
            services.AddTransient<IPrescriptionHistoryService, PrescriptionHistoryService>();
            
            services.AddTransient<IMedicationValidator, MedicationValidator>();
            services.AddTransient<IValidator<PrescriptionItemCreateDto>, PrescriptionItemCreateValidator>();
            services.AddTransient<IValidator<PrescriptionItemUpdateDto>, PrescriptionItemUpdateValidator>();
            return services;
        }

        /// <summary>
        /// Add Configuration for mass transit
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddPrescriptionMassTransitConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(a =>
            {
                a.AddEntityFrameworkOutbox<PrescriptionDbContext>(opt =>
                {
                    opt.QueryDelay = TimeSpan.FromSeconds(100);
                    opt.UsePostgres();
                    opt.UseBusOutbox();
                });
                a.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("prescription", false));
                a.UsingRabbitMq((context, config) =>
                {
                    var rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";
                    var rabbitMqPort = configuration["RabbitMQ:Port"] ?? "5672";
                    var rabbitMqUser = configuration["RabbitMQ:Username"] ?? "guest";
                    var rabbitMqPass = configuration["RabbitMQ:Password"] ?? "guest";
                    var rabbitMqUri = $"amqp://{rabbitMqUser}:{rabbitMqPass}@{rabbitMqHost}:{rabbitMqPort}";
                    config.Host(new Uri(rabbitMqUri));
                    config.ConfigureEndpoints(context);
                });
            });

            return services;

        }
    }
}
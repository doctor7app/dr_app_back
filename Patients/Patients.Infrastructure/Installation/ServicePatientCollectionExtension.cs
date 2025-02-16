using Common.Extension.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patients.Application.Interfaces;
using Patients.Infrastructure.Implementation;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Installation
{
    public static class ServicePatientCollectionExtension
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
            services.AddAutoMapperConfiguration(applicationAssembly, infrastructureAssembly);

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
                a.AddEntityFrameworkOutbox<PatientDbContext>(opt =>
                {
                    opt.QueryDelay =  TimeSpan.FromSeconds(100);
                    opt.UsePostgres();
                    opt.UseBusOutbox();
                });
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
                });
            });

            return services;

        }
    }
}
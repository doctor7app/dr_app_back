using Common.Extension;
using Microsoft.Extensions.DependencyInjection;
using Patients.Application;
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
            services.AddAutoMapperConfiguration();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IAdresseService, AdresseService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IMedicalInfoService, MedicalInfoService>();
            return services;
        }
    }
}
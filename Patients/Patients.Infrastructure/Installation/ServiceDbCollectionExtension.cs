using Common.Services.Implementation;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Installation
{
    public static class ServiceDbCollectionExtension
    {
        /// <summary>
        /// Add reference to database Context and Asp.net Identity
        /// Without asp.net identity provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="migrationName"></param>
        /// <returns></returns>
        public static IServiceCollection AddPatientDatabaseServiceCollection(this IServiceCollection services, string migrationName)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            string strConnection = builder.Build().GetConnectionString("MyDbPost");
            services.AddDbContext<PatientDbContext>(options =>
            {
                options.UseNpgsql(strConnection, sql => sql.MigrationsAssembly(migrationName));
            }, ServiceLifetime.Transient);
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IServiceGeneric<,,,>), typeof(ServiceGeneric<,,,>));
            return services;
        }

        /// <summary>
        /// Seed Data For Domain Models
        /// </summary>
        /// <param name="app"></param>
        public static void UseInitializeDbDomain(this IApplicationBuilder app)
        {
            DbInitializer.InitializeDomain(app);
        }
        
    }
}
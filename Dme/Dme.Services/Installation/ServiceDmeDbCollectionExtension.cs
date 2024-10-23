using Common.Services.Implementation;
using Common.Services.Interfaces;
using Dme.Infrastructure.Persistence;
using Dme.Services.Implementation;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dme.Services.Installation
{
    public static class ServiceDmeDbCollectionExtension
    {
        /// <summary>
        /// Add reference to database Context 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="migrationName"></param>
        /// <returns></returns>
        public static IServiceCollection AddDmeDatabaseServiceCollection(this IServiceCollection services, string migrationName)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            string strConnection = builder.Build().GetConnectionString("MyDbPost");
            services.AddDbContext<DmeDbContext>(options =>
            {
                options.UseNpgsql(strConnection, sql => sql.MigrationsAssembly(migrationName));
            }, ServiceLifetime.Transient);
            
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IServiceGeneric<,,>), typeof(ServiceGeneric<,,,>));
            
            services.AddScoped<IDmeService, DmeService>();
            services.AddScoped<IConsultationService, ConsultationService>();
            services.AddScoped<IDiagnosticService, DiagnosticService>();
            services.AddScoped<ITreatmentService, TreatmentService>();
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
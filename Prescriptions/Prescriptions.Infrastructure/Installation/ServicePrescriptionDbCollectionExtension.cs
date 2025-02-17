﻿using Common.Services.Implementation;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Installation
{
    public static class ServicePrescriptionDbCollectionExtension
    {
        /// <summary>
        /// Add reference to database Context and Asp.net Identity
        /// Without asp.net identity provider.
        /// </summary>
        /// <param name="services"></param>=
        /// <returns></returns>
        public static IServiceCollection AddPrescriptionDatabaseServiceCollection(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var strConnection = builder.Build().GetConnectionString("MyDbPost");
            services.AddDbContext<PrescriptionDbContext>(options =>
            {
                options.UseNpgsql(strConnection, sql => sql.MigrationsAssembly("Prescriptions.Infrastructure"));
            }, ServiceLifetime.Transient);

            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddTransient(typeof(IServiceGeneric<,,,,>), typeof(ServiceGeneric<,,,,>));
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
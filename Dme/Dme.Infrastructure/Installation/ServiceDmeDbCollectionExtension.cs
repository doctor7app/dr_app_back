﻿using Common.Services.Implementation;
using Common.Services.Interfaces;
using Dme.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dme.Infrastructure.Installation
{
    public static class ServiceDmeDbCollectionExtension
    {
        /// <summary>
        /// Add reference to database Context 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDmeDatabaseServiceCollection(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            string strConnection = builder.Build().GetConnectionString("MyDbPost");
            services.AddDbContext<DmeDbContext>(options =>
            {
                options.UseNpgsql(strConnection, sql => sql.MigrationsAssembly("Dme.Infrastructure"));
            }, ServiceLifetime.Transient);
            
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped(typeof(IServiceGeneric<,,,,>), typeof(ServiceGeneric<,,,,>));
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
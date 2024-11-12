using Dme.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dme.Infrastructure.Installation
{
    public class DbInitializer
    {
		public static void InitializeDomain(IApplicationBuilder app)
		{
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DmeDbContext>();
            if (context == null) return;
            MigrateDb(context);
            InitDatabaseDomain(context);
            context.Dispose();
        }
        

        /// <summary>
        /// check if database Exist or Not.
        /// If database does not exist it will create it.
        /// If there is any pending migrations it will update the database.
        /// </summary>
        /// <param name="context"></param>
        private static void MigrateDb(DmeDbContext context)
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            else
            {
                context.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// Initialize the database when there is no data 
        /// </summary>
        public static  void InitDatabaseDomain(DmeDbContext dbContext)
        {
            try
            {
                if (!dbContext.Dmes.Any())
                {
                    
                }
                
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                if (e.InnerException!=null) throw;
                throw;
            }
        }
    }
}
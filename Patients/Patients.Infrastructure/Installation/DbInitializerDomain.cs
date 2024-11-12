using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Installation
{
    public class DbInitializer
    {
		public static void InitializeDomain(IApplicationBuilder app)
		{
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<PatientDbContext>();
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
        private static void MigrateDb(PatientDbContext context)
        {
            // Check if the database exists
            if (context.Database.GetPendingMigrations().Any())
            {
                // Apply pending migrations
                context.Database.Migrate();
            }
            else
            {
                // Ensure the database is created
                context.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// Initialize the database when there is no data 
        /// </summary>
        public static  void InitDatabaseDomain(PatientDbContext dbContext)
        {
            try
            {
                if (!dbContext.Patients.Any())
                {
                    
                }
                
                if (!dbContext.Contacts.Any())
                {
                    
                }
                
                if (!dbContext.Adresses.Any())
                {
                    
                }
                
                if (!dbContext.MedicalInformation.Any())
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
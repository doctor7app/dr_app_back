using Microsoft.EntityFrameworkCore;

namespace DocumentManagement.Infrastructure.Data
{
    public class DbInitializer
    {
        public static void InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetService<DocumentDbContext>()
                ?? throw new InvalidOperationException("Failed to retrieve DocumentDbContext from the service provider.");
            SeedData(context);
        }

        private static void SeedData(DocumentDbContext context)
        {
            context.Database.Migrate();
        }
    }
}

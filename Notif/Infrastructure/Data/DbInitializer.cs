using Microsoft.EntityFrameworkCore;

namespace Notif.Infrastructure.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetService<DataDbContext>()
            ?? throw new InvalidOperationException("Failed to retrieve DbContext from the service provider.");
        SeedData(context);
    }

    private static void SeedData(DbContext context)
    {
        context.Database.Migrate();
    }
}

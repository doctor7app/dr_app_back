using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Infrastructure.Extension;

public static class DbContextExtension
{
    public static void AddDbContext<TDbContext>(
        this IServiceCollection services
        , IConfiguration configuration
        )
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("MyDbPost")));
    }
}

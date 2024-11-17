using Common.Infrastructure.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Infrastructure.Installation;

public static class HostBuilderExtension
{
    public static IHostBuilder UseCommonSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().ConfigureSerilog(configuration).CreateLogger();
        return hostBuilder.UseSerilog();
    }
}

using Serilog;
using Serilog.Events;

namespace Dme.Api.Extensions;

public static class SerilogExtensions
{
    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration loggerConfiguration, string appName, string seqUrl)
    {
        loggerConfiguration
            .MinimumLevel.Debug() // Default: capture Debug and higher
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress too many framework logs
            .Enrich.FromLogContext()
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .Enrich.WithProperty("ThreadId", Environment.CurrentManagedThreadId)
            .Enrich.WithProperty("Application", appName)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate
            )
            .WriteTo.Debug() 
            .WriteTo.OpenTelemetry(options =>
            {
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = appName
                };
            })
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7); // Rolling daily log files
        if (seqUrl != null)
        {
            loggerConfiguration
            .WriteTo.Seq(seqUrl);
        }
        return loggerConfiguration;   
    }
}

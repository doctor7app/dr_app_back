using Serilog;
using Serilog.Events;

namespace DocumentManagement.Infrastructure.Logging;

public class SerilogConfig
{
    public static LoggerConfiguration CreateLoggerConfiguration()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug() // Default: capture Debug and higher
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Suppress too many framework logs
            .Enrich.FromLogContext() // Add extra context to each log
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Literate
            )
            .WriteTo.Debug() // For Visual Studio or Rider output window
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7); // Rolling daily log files
    }
}

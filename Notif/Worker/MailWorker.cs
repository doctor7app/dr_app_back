using Common.Contracts.Notif;
using Microsoft.Extensions.Logging;
using Notif.Core.Interfaces.Services;
using Serilog.Context;

namespace Notif.Worker
{
    public class MailWorker(IServiceProvider serviceProvider, ILogger<MailWorker> logger) : BackgroundService
    {
        private readonly TimeSpan _highUrgencyInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _mediumUrgencyInterval = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _lowUrgencyInterval = TimeSpan.FromMinutes(5);
        private INotifService _notifService;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            _notifService = scope.ServiceProvider.GetRequiredService<INotifService>();
            var tasks = new[]
            {
                RunLowUrgencyTask(stoppingToken),
                RunMediumUrgencyTask(stoppingToken),
                RunHighUrgencyTask(stoppingToken)
            };
            await Task.WhenAll(tasks);
        }

        private async Task SendNotificationsAsync(NotifPriority priority)
        {
            try
            {
                var priorityName = Enum.GetName(typeof(NotifPriority), priority);
                logger.LogDebug("Start sending emails priority : {priority}", priorityName);
                await _notifService.SendEmailAsync(priority);
                logger.LogDebug("End sending emails priority : {priority}", priorityName);
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("Error", e.StackTrace, true))
                {
                    logger.LogError(e, "Exception occurred: {Message}", e.Message);
                }
            }
        }

        private async Task RunLowUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_lowUrgencyInterval, stoppingToken);
                if (!stoppingToken.IsCancellationRequested)
                {
                    await SendNotificationsAsync(NotifPriority.Low);
                }
            }
        }

        private async Task RunMediumUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_mediumUrgencyInterval, stoppingToken);
                if (!stoppingToken.IsCancellationRequested)
                {
                    await SendNotificationsAsync(NotifPriority.Medium);
                }
            }
        }

        private async Task RunHighUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_highUrgencyInterval, stoppingToken);
                if (!stoppingToken.IsCancellationRequested)
                {
                    await SendNotificationsAsync(NotifPriority.High);
                }
            }
        }
    }
}

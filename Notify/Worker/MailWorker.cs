using Common.Contracts.Notif;
using Notify.Core.Interfaces.Services;
using Serilog.Context;

namespace Notify.Worker
{
    public class MailWorker(IServiceProvider serviceProvider, ILogger<MailWorker> logger) : BackgroundService
    {
        private readonly TimeSpan _highUrgencyInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _mediumUrgencyInterval = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _lowUrgencyInterval = TimeSpan.FromMinutes(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tasks = new[]
            {
                RunLowUrgencyTask(stoppingToken),
                RunMediumUrgencyTask(stoppingToken),
                RunHighUrgencyTask(stoppingToken)
            };
            await Task.WhenAll(tasks);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping mail worker...");
            await base.StopAsync(cancellationToken);
            logger.LogInformation("Mail worker stopped.");
        }

        private async Task SendNotificationsAsync(NotifPriority priority)
        {
            try
            {
                var priorityName = Enum.GetName(typeof(NotifPriority), priority);
                logger.LogDebug("Start sending emails priority : {priority}", priorityName);
                using var scope = serviceProvider.CreateScope();
                var notifyService = scope.ServiceProvider.GetRequiredService<INotifyService>();
                await notifyService.SendNotificationsAsync(priority);
                logger.LogDebug("End sending emails priority : {priority}", priorityName);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception occurred: {Message}", e.Message);
            }
        }

        private async Task RunLowUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_lowUrgencyInterval, stoppingToken);
                await SendNotificationsAsync(NotifPriority.Low);
            }
        }

        private async Task RunMediumUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_mediumUrgencyInterval, stoppingToken);
                await SendNotificationsAsync(NotifPriority.Medium);
            }
        }

        private async Task RunHighUrgencyTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_highUrgencyInterval, stoppingToken);
                await SendNotificationsAsync(NotifPriority.High);
            }
        }
    }
}

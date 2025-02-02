using Common.Contracts.Notif;
using MassTransit;
using Notify.Core.Interfaces.Services;

namespace Notify.Consumers;

public class NotifyConsumer(IServiceProvider serviceProvider) : IConsumer<NotifRequest>
{
    public async Task Consume(ConsumeContext<NotifRequest> context)
    {
        var message = context.Message;
        using (var scope = serviceProvider.CreateScope())
        {
            var notifyService = scope.ServiceProvider.GetRequiredService<INotifyService>();
            await notifyService.SaveNotificationAsync(message);
        }
        await context.RespondAsync(new
        {
            Status = "Received",
            Message = "The email request has been accepted and will be processed shortly.",
            RequestId = message.CorrelationId
        });
    }
}

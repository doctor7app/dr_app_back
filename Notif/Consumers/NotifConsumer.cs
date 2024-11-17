using Common.Contracts.Notification.V1;
using MassTransit;
using Notif.Core.Interfaces.Services;

namespace Notif.Consumers;

public class NotifConsumer(INotifService notifService) : IConsumer<NotifMessage>
{
    public async Task Consume(ConsumeContext<NotifMessage> context)
    {
        await notifService.ProcessNotificationAsync(context.Message);
    }
}

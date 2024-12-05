using Common.Contracts.Notif;
using MassTransit;
using Notif.Core.Interfaces.Services;

namespace Notif.Consumers;

public class NotifConsumer(IServiceProvider serviceProvider) : IConsumer<NotifRequest>
{
    public async Task Consume(ConsumeContext<NotifRequest> context)
    {
        var message = context.Message;
        using (var scope = serviceProvider.CreateScope())
        {
            var notifService = scope.ServiceProvider.GetRequiredService<INotifService>();
            await notifService.SaveNotificationAsync(message);
        }
        await context.RespondAsync(new
        {
            Status = "Received",
            Message = "The email request has been accepted and will be processed shortly.",
            RequestId = message.CorrelationId
        });
    }
}

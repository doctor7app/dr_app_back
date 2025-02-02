using Common.Contracts.Notif;
using Notify.Core.Models;

namespace Notify.Core.Interfaces.Services;

public interface INotifyService
{
    Task SaveNotificationAsync(NotifRequest notifyRequest);
    Task ProcessNotificationAsync(NotifItem notification, CancellationToken token);
    Task SendNotificationsAsync(NotifPriority priority,int batch = 10);
}

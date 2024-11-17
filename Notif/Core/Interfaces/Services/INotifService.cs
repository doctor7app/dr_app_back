using Common.Contracts.Notification.V1;

namespace Notif.Core.Interfaces.Services;

public interface INotifService
{
    Task ProcessNotificationAsync(NotifMessage message);
}

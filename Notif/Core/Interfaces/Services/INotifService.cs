using Common.Contracts.Notif;
using Notif.Core.Models;

namespace Notif.Core.Interfaces.Services;

public interface INotifService
{
    Task SaveNotificationAsync(NotifRequest notifRequest);
    Task ProcessNotificationAsync(NotifItem notification);
    Task SendEmailAsync(NotifPriority priority,int batch = 10);
}

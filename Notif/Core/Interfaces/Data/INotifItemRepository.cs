using Common.Contracts.Notif;
using Notif.Core.Models;

namespace Notif.Core.Interfaces.Data;

public interface INotifItemRepository
{
    Task<NotifItem> GetByIdAsync(Guid id);
    Task<IEnumerable<NotifItem>> GetByAsync(NotifType type, NotifStatus status, NotifPriority priority, int limit);
    Task AddAsync(NotifItem notif);
    Task UpdateAsync(NotifItem notif);
    Task UpdateAsync(IEnumerable<NotifItem> notifs);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(IEnumerable<NotifItem> notifs);
    Task DeleteOldAsync(int hours);
}

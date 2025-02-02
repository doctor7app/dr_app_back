using Common.Contracts.Notif;
using Notify.Core.Models;

namespace Notify.Core.Interfaces.Data;

public interface INotifyItemRepository
{
    Task<NotifItem> GetByIdAsync(Guid id);
    Task<IEnumerable<NotifItem>> GetByAsync(NotifType type, NotifStatus status, NotifPriority priority, int limit);
    Task AddAsync(NotifItem notification);
    Task UpdateAsync(NotifItem notification);
    Task UpdateAsync(IEnumerable<NotifItem> notifications);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(IEnumerable<NotifItem> notifications);
    Task DeleteAsync(NotifItem notification);
    Task DeleteOldAsync(int hours);
}

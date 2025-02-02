using Common.Contracts.Notif;
using Microsoft.EntityFrameworkCore;
using Notify.Core.Interfaces.Data;
using Notify.Core.Models;
using Notify.Infrastructure.Data;

namespace Notify.Infrastructure.Repositories;

public class NotifyItemRepository(DataDbContext context) : INotifyItemRepository
{
    public async Task<NotifItem> GetByIdAsync(Guid id)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(NotifItem notification)
    {
        await context.Notifications.AddAsync(notification);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NotifItem notification)
    {
        context.Notifications.Update(notification);
        await context.SaveChangesAsync();
    }

    async Task INotifyItemRepository.UpdateAsync(IEnumerable<NotifItem> notifications)
    {
        foreach (var notification in notifications)
        {
            context.Notifications.Update(notification);
        }
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
        {
            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(IEnumerable<NotifItem> notifications)
    {
        var notificationItems = notifications.ToList();
        if (notificationItems.Count != 0)
        {
            context.Notifications.RemoveRange(notificationItems);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(NotifItem notification)
    {
        if (notification != null)
        {
            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteOldAsync(int hours)
    {
        var oldRecords = context.Notifications
                        .Where(n => n.CreatedAt < DateTime.UtcNow.AddHours(-hours))
                        .Where(n => n.Status == NotifStatus.Sent);
        context.Notifications.RemoveRange(oldRecords);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<NotifItem>> GetByAsync(
        NotifType type
        , NotifStatus status
        , NotifPriority priority
        , int limit
        )
    {
        limit = Math.Max(limit, 1);

        return await context.Notifications
        .OrderBy(n => n.CreatedAt)
        .Where(n => n.Type == type)
        .Where(n => n.Status == status)
        .Where(n => n.Priority == priority)
        .Take(limit)            
        .ToListAsync();
    }
}

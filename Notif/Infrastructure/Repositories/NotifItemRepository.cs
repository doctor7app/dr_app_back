using Common.Contracts.Notif;
using Microsoft.EntityFrameworkCore;
using Notif.Core.Interfaces.Data;
using Notif.Core.Models;
using Notif.Infrastructure.Data;

namespace Notif.Infrastructure.Repositories;

public class NotifItemRepository(DataDbContext context) : INotifItemRepository
{
    public async Task<NotifItem> GetByIdAsync(Guid id)
    {
        return await context.Notifications
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(NotifItem notif)
    {
        await context.Notifications.AddAsync(notif);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NotifItem notif)
    {
        context.Notifications.Update(notif);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(IEnumerable<NotifItem> notifs)
    {
        foreach (var notif in notifs)
        {
            context.Notifications.Update(notif);
        }
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notif = await GetByIdAsync(id);
        if (notif != null)
        {
            context.Notifications.Remove(notif);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(IEnumerable<NotifItem> notifs)
    {
        if (notifs.Any())
        {
            context.Notifications.RemoveRange(notifs);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(NotifItem notif)
    {
        if (notif != null)
        {
            context.Notifications.Remove(notif);
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

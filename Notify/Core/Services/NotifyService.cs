using AutoMapper;
using Common.Contracts.Notif;
using Notify.Core.Interfaces.Data;
using Notify.Core.Interfaces.Services;
using Notify.Core.Models;

namespace Notify.Core.Services;

public class NotifyService(ILogger<NotifyService> logger, IEmailService emailService, IMapper mapper, INotifyItemRepository notifyItemRepository) : INotifyService
{
    public async Task SaveNotificationAsync(NotifRequest notifyRequest)
    {
        var notification = mapper.Map<NotifItem>(notifyRequest);
        notification.Status = NotifStatus.Pending;
        await notifyItemRepository.AddAsync(notification);
    }

    public async Task SendNotificationsAsync(NotifPriority priority, int batch = 10)
    {
        var notifications = await notifyItemRepository.GetByAsync(NotifType.Email,NotifStatus.Pending, priority, batch);
        var tasks = notifications.Select(notification => ProcessNotificationAsync(notification, CancellationToken.None));
        await Task.WhenAll(tasks);
    }

    public async Task ProcessNotificationAsync(NotifItem notification, CancellationToken token)
    {
        try
        {
            notification.Status = NotifStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            switch (notification.Type)
            {
                case NotifType.Email:
                    await emailService.SendEmailAsync(notification, token);
                    break;
                case NotifType.SMS:
                    break;
                case NotifType.Push:
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            notification.ErrorMessage = ex.Message;
            notification.SentAt = null;
            notification.Status = NotifStatus.Failed;

            logger.LogError(ex, "Exception occurred while processing notification: {Message}", ex.Message);
        }
        finally
        {
            await notifyItemRepository.UpdateAsync(notification);
        }
    }
}


using AutoMapper;
using Common.Contracts.Notif;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notif.Core.Interfaces.Data;
using Notif.Core.Interfaces.Services;
using Notif.Core.Models;

namespace Notif.Core.Services;

public class NotifService(IMapper mapper, INotifItemRepository notifItemRepository) : INotifService
{
    public async Task SaveNotificationAsync(NotifRequest notifRequest)
    {
        var notification = mapper.Map<NotifItem>(notifRequest);
        notification.Status = NotifStatus.Pending;
        await notifItemRepository.AddAsync(notification);
    }
    public async Task SendEmailAsync(NotifPriority priority, int batch = 10)
    {
        var notifications = await notifItemRepository.GetByAsync(NotifType.Email,NotifStatus.Pending, priority, batch);
        foreach (var notification in notifications) {
            await ProcessNotificationAsync(notification);
        }
    }

    public async Task ProcessNotificationAsync(NotifItem notification)
    {
        try
        {
            notification.Status = NotifStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
            switch (notification.Type)
            {
                case NotifType.Email:
                    await SendEmailAsync(notification);
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
        }
        finally
        {
            await notifItemRepository.UpdateAsync(notification);
        }
    }

    private async Task SendEmailAsync(NotifItem notification)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Notification Service", "noreply@yourapp.com"));
        email.To.Add(MailboxAddress.Parse(notification.Recipient));
        email.Subject = notification.Subject;
        email.Body = new TextPart("plain") { Text = notification.Content };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.mailtrap.io", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("username", "password");
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
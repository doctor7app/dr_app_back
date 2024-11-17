

using Common.Contracts.Notification.V1;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Notif.Core.Interfaces.Services;
using Notif.Core.Models;
using Notif.Infrastructure.Data;

namespace Notif.Core.Services;

public class NotifService(DataDbContext context) : INotifService
{
    public async Task ProcessNotificationAsync(NotifMessage message)
    {
        var notification = new NotifItem
        {
            Recipient = message.Recipient,
            Message = message.Message,
            Type = message.Type,
            Priority = message.Priority,
            Status = NotifStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            // Simulate sending a notification
            await SendEmailAsync(notification.Recipient, notification.Message);
            notification.Status = NotifStatus.Sent;
            notification.SentAt = DateTime.UtcNow;
        }
        catch
        {
            notification.Status = NotifStatus.Failed;
        }
        finally
        {
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
        }
    }

    private async Task SendEmailAsync(string recipient, string message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Notification Service", "noreply@yourapp.com"));
        email.To.Add(MailboxAddress.Parse(recipient));
        email.Subject = "Notification";
        email.Body = new TextPart("plain") { Text = message };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.mailtrap.io", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("username", "password");
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
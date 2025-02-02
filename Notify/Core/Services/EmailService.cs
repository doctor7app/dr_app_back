using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notify.Core.Interfaces.Services;
using Notify.Core.Models;
using Notify.Core.Options;

namespace Notify.Core.Services;
public class EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings) : IEmailService
{
    public async Task SendEmailAsync(NotifItem notification, CancellationToken token)
    {
        try
        {
            var emailSettingsValue = emailSettings.Value;

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(emailSettingsValue.SmtpName, emailSettingsValue.SmtpFrom));
            email.To.Add(MailboxAddress.Parse(notification.Recipient));
            email.Subject = notification.Subject;
            email.Body = new TextPart("plain") { Text = notification.Content };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(emailSettingsValue.SmtpHost, emailSettingsValue.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable, token);
            if (!string.IsNullOrEmpty(emailSettingsValue.SmtpUser) && !string.IsNullOrEmpty(emailSettingsValue.SmtpPass))
            {
                await smtpClient.AuthenticateAsync(emailSettingsValue.SmtpUser, emailSettingsValue.SmtpPass, token);
            }
            await smtpClient.SendAsync(email, token);
            await smtpClient.DisconnectAsync(true, token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Recipient}", notification.Recipient);
            throw;
        }
    }
}
using Notify.Core.Models;

namespace Notify.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(NotifItem notification, CancellationToken token);
}

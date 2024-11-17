using Common.Contracts.Notification.V1;

namespace Notif.Core.Models;

public class NotifItem
{
    public Guid Id { get; set; }
    public string Recipient { get; set; }
    public string Message { get; set; }
    public NotifType Type { get; set; }
    public NotifPriority Priority { get; set; }
    public NotifStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

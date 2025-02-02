using Common.Contracts.Notif;

namespace Notify.Core.Models;

public class NotifItem
{
    public Guid Id { get; set; }
    public Guid CorrelationId { get; set; }
    public string SenderService { get; set; }
    public NotifType Type { get; set; }
    public NotifStatus Status { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public NotifPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public string ErrorMessage { get; set; }
}

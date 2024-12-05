namespace Common.Contracts.Notif;

public class NotifRequest
{
    public Guid CorrelationId { get; set; }
    public string SenderService { get; set; }
    public NotifType Type { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public NotifPriority Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

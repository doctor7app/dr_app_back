namespace Common.Contracts.Notification.V1;

public class NotifMessage
{
    /// <summary>
    /// The type of notification to be sent (Email, SMS, Push, etc.).
    /// </summary>
    public NotifType Type { get; set; }

    /// <summary>
    /// The recipient's address (email, phone number, etc.).
    /// </summary>
    public string Recipient { get; set; }

    /// <summary>
    /// The content of the notification.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// An optional correlation ID to trace the message across services.
    /// </summary>
    public Guid CorrelationId { get; set; }

    /// <summary>
    /// Priority of the notification (e.g., Low, Medium, High).
    /// </summary>
    public NotifPriority Priority { get; set; }
}

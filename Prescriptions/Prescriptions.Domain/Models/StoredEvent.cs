using Common.Enums.Prescriptions;

namespace Prescriptions.Domain.Models;

/// <summary>
/// The event that’s persisted in the database for historical tracking and replay.
/// </summary>
public class StoredEvent
{
    public Guid Id { get; set; }
    public PrescriptionEventType EventType { get; set; }
    public string Data { get; set; }
    public DateTime OccurredOn { get; set; }
    /// <summary>
    /// Link back to the aggregate (Prescription or PrescriptionItem)
    /// </summary>
    public Guid AggregateId { get; set; }
    /// <summary>
    /// To distinguish between Prescription and PrescriptionItem
    /// </summary>
    public string AggregateType { get; set; }  
}
using Prescriptions.Domain.Interfaces;

namespace Prescriptions.Domain.Abstract;

/// <summary>
/// Represents real-time domain events
/// </summary>
public abstract class PrescriptionEvent : IPrescriptionEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
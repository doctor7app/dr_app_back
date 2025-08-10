
namespace Prescriptions.Domain.Interfaces;

public interface IPrescriptionEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
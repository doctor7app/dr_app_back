using Prescriptions.Domain.Interfaces;

namespace Prescriptions.Application.Interfaces;

public interface IEventStoreService
{
    void SaveEvents(IEnumerable<IPrescriptionEvent> events);
}
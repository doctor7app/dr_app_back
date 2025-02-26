using Prescriptions.Domain.Interfaces;

namespace Prescriptions.Application.Interfaces.Services;

public interface IEventStoreService
{
    void SaveEvents(IEnumerable<IPrescriptionEvent> events);
}
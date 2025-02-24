using Prescriptions.Domain.Interfaces;

namespace Prescriptions.Domain.Abstract;

public abstract class AggregateRoot
{
    private readonly List<IPrescriptionEvent> _events = new();
    public IReadOnlyCollection<IPrescriptionEvent> Events => _events.AsReadOnly();

    protected void AddEvent(IPrescriptionEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void ClearEvents() => _events.Clear();
}

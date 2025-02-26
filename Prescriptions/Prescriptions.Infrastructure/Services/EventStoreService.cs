using System.Text.Json;
using Common.Enums.Prescriptions;
using Common.Services.Interfaces;
using Prescriptions.Application.Interfaces.Services;
using Prescriptions.Domain.Events;
using Prescriptions.Domain.Interfaces;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Services;

public class EventStoreService : IEventStoreService
{
    private readonly IRepository<StoredEvent, PrescriptionDbContext> _repository;

    public EventStoreService(IRepository<StoredEvent, PrescriptionDbContext> repository)
    {
        _repository = repository;
    }

    public async void SaveEvents(IEnumerable<IPrescriptionEvent> events)
    {
        foreach (var ev in events)
        {
            var storedEvent = new StoredEvent
            {
                Id = Guid.NewGuid(),
                EventType = GetEventType(ev),
                Data = JsonSerializer.Serialize(ev, ev.GetType()),
                OccurredOn = ev.OccurredOn,
                AggregateId = GetAggregateId(ev),
                AggregateType = GetAggregateType(ev)
            };

           await _repository.AddAsync(storedEvent);
        }

        await _repository.Complete();
    }

    private static PrescriptionEventType GetEventType(IPrescriptionEvent ev)
    {
        return ev switch
        {
            PrescriptionCreatedEvent => PrescriptionEventType.PrescriptionCreated,
            PrescriptionUpdatedEvent => PrescriptionEventType.PrescriptionUpdated,
            PrescriptionDeletedEvent => PrescriptionEventType.PrescriptionDeleted,
            PrescriptionItemCreatedEvent => PrescriptionEventType.ItemCreated,
            PrescriptionItemUpdatedEvent => PrescriptionEventType.ItemUpdated,
            PrescriptionItemDeletedEvent => PrescriptionEventType.ItemDeleted,
            _ => throw new ArgumentException("Unknown event type")
        };
    }

    private static Guid GetAggregateId(IPrescriptionEvent ev)
    {
        return ev switch
        {
            PrescriptionCreatedEvent e => e.PrescriptionId,
            PrescriptionUpdatedEvent e => e.PrescriptionId,
            PrescriptionDeletedEvent e => e.PrescriptionId,
            PrescriptionItemCreatedEvent e => e.PrescriptionItemId,
            PrescriptionItemUpdatedEvent e => e.PrescriptionItemId,
            PrescriptionItemDeletedEvent e => e.PrescriptionItemId,
            _ => throw new ArgumentException("Unknown event type")
        };
    }

    private static string GetAggregateType(IPrescriptionEvent ev)
    {
        return ev switch
        {
            PrescriptionCreatedEvent _ => "Prescription",
            PrescriptionUpdatedEvent _ => "Prescription",
            PrescriptionDeletedEvent _ => "Prescription",
            PrescriptionItemCreatedEvent _ => "PrescriptionItem",
            PrescriptionItemUpdatedEvent _ => "PrescriptionItem",
            PrescriptionItemDeletedEvent _ => "PrescriptionItem",
            _ => throw new ArgumentException("Unknown event type")
        };
    }
}

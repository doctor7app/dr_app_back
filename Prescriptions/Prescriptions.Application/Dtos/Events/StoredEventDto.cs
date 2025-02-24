using AutoMapper;
using Common.Enums.Prescriptions;
using Common.Interfaces;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Events;


public class StoredEventDto : IMapFrom<StoredEvent> 
{
    public Guid Id { get; set; }
    public PrescriptionEventType EventType { get; set; }
    public string Data { get; set; }  // You may want to deserialize this depending on your use case
    public DateTime OccurredOn { get; set; }

    // Optionally, you could also include aggregate info
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<StoredEventDto, StoredEvent>()
            .ReverseMap();
    }
}

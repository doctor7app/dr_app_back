using System.Text.Json;
using AutoMapper;
using Common.Services.Interfaces;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Interfaces;
using Prescriptions.Domain.Event;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Implementation;

public class PrescriptionEventService : IPrescriptionEventService
{
    private readonly IRepository<PrescriptionEvent, PrescriptionDbContext> _repository;
    private readonly IMapper _mapper;

    public PrescriptionEventService(
        IRepository<PrescriptionEvent, PrescriptionDbContext> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<PrescriptionEventDto>> GetEventsByPrescriptionAsync(Guid prescriptionId)
    {
        var events = await _repository.GetListAsync(
            e => e.FkPrescriptionId == prescriptionId
             //q => q.OrderByDescending(e => e.Timestamp)
        );
        events = events.OrderBy(a => a.Timestamp);
        return _mapper.Map<List<PrescriptionEventDto>>(events);
    }

    public async Task<PrescriptionEventDetailDto> GetEventDetailsAsync(Guid eventId)
    {
        var eventEntity = await _repository.GetAsync(e => e.PrescriptionEventId == eventId);
        if (eventEntity == null) return null;

        return new PrescriptionEventDetailDto
        {
            Id = eventEntity.PrescriptionEventId,
            EventType = eventEntity.EventType,
            Timestamp = eventEntity.Timestamp,
            DoctorId = eventEntity.FkDoctorId,
            EventDataJson = eventEntity.EventDataJson,
            PrescriptionId = eventEntity.FkPrescriptionId,
            EventData = JsonSerializer.Deserialize<object>(eventEntity.EventDataJson)
        };
        
    }
}
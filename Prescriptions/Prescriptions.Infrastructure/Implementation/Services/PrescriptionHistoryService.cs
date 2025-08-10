using AutoMapper;
using Common.Services.Interfaces;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Interfaces.Services;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Implementation.Services;

public class PrescriptionHistoryService : IPrescriptionHistoryService
{
    private readonly IRepository<StoredEvent, PrescriptionDbContext> _repository;
    private readonly IMapper _mapper;

    public PrescriptionHistoryService(
        IRepository<StoredEvent, PrescriptionDbContext> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


    public async Task<IEnumerable<StoredEventDto>> GetPrescriptionHistoryAsync(Guid prescriptionId)
    {
        var events = await _repository.GetListAsync(
            x => x.AggregateId == prescriptionId && x.AggregateType == "Prescription");

        return _mapper.Map<IEnumerable<StoredEventDto>>(events);
    }

    public async Task<IEnumerable<StoredEventDto>> GetPrescriptionItemHistoryAsync(Guid prescriptionItemId)
    {
        var events = await _repository.GetListAsync(
            x => x.AggregateId == prescriptionItemId && x.AggregateType == "PrescriptionItem");

        return _mapper.Map<IEnumerable<StoredEventDto>>(events);
    }
}
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces;
using Prescriptions.Domain.Event;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;
using static Grpc.Core.Metadata;

namespace Prescriptions.Infrastructure.Implementation;

public class PrescriptionService : IPrescriptionService
{
    private readonly IRepository<Prescription, PrescriptionDbContext> _work;
    private readonly IRepository<PrescriptionEvent, PrescriptionDbContext> _workEvent;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public PrescriptionService(IRepository<Prescription, PrescriptionDbContext> work,
        IRepository<PrescriptionEvent, PrescriptionDbContext> workEvent,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _work = work;
        _workEvent = workEvent;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionAsync()
    {
        var obj = await _work.GetListAsync(includes:
            a => a.Include(z => z.Items));
        return _mapper.Map<IEnumerable<PrescriptionDto>>(obj);
    }

    public async Task<PrescriptionDto> GetPrescriptionByIdAsync(Guid id)
    {
        if (id.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PrescriptionId == id, includes:
            a => a.Include(z => z.Items));

        return _mapper.Map<PrescriptionDto>(obj);
    }

    public async Task<PrescriptionDetailsDto> GetPrescriptionDetailsAsync(Guid id)
    {
        if (id.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PrescriptionId == id, includes:
            a => a.Include(z => z.Items)
                .Include(z=>z.DomainEvents));

        return _mapper.Map<PrescriptionDetailsDto>(obj);
    }

    public async Task<bool> CreatePrescriptionAsync(PrescriptionCreateDto dto)
    {
        if (dto.DoctorId.IsNullOrEmptyGuid() || dto.PatientId.IsNullOrEmptyGuid() ||
            dto.ConsultationId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var item = _mapper.Map<Prescription>(dto);
        await _work.AddAsync(item);

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Prescription to database");
        }

        return true;
    }

    public async Task<bool> UpdatePrescriptionAsync(Guid id, Delta<PrescriptionUpdateDto> patch)
    {
        if (id.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var entityToUpdate = await _work.GetAsync(z => z.PrescriptionId == id);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {id} n'existe pas dans la base de données!");
        }
        var dto = new PrescriptionUpdateDto();
        patch.Patch(dto);

        _mapper.Map(dto, entityToUpdate);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not update Prescription to database");
        }
        return true;
    }

    public async Task<bool> DeletePrescriptionAsync(Guid id)
    {
        if (id.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.PrescriptionId == id);
        if (obj == null)
        {
            throw new Exception($"L'élement avec l'id {id} n'existe pas dans la base de données!");
        }
        _work.Remove(obj);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Patient from database");
        }
        return true;
    }

    public async Task<bool> AddPrescriptionEventAsync(Guid prescriptionId, PrescriptionEventDto eventDto)
    {
        if (prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var item = _mapper.Map<PrescriptionEvent>(eventDto);
        await _workEvent.AddAsync(item);

        var result = await _workEvent.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Prescription to database");
        }

        return true;
    }
}
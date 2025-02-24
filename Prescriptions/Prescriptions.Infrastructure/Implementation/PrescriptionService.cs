using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Implementation;

public class PrescriptionService : IPrescriptionService
{
    private readonly IRepository<Prescription, PrescriptionDbContext> _work;
    private readonly IEventStoreService _eventStoreService;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public PrescriptionService(IRepository<Prescription, PrescriptionDbContext> work,
        IEventStoreService eventStoreService,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _work = work;
        _eventStoreService = eventStoreService;
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
    

    public async Task<bool> CreatePrescriptionAsync(PrescriptionCreateDto dto)
    {
        if (dto.DoctorId.IsNullOrEmptyGuid() || dto.PatientId.IsNullOrEmptyGuid() ||
            dto.ConsultationId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var prescription = _mapper.Map<Prescription>(dto);
        prescription.PrescriptionId = Guid.NewGuid();

        //Trigger the Domain Event
        foreach (var item in dto.Items)
        {
            var newItem = new PrescriptionItem
            {
                PrescriptionItemId = Guid.NewGuid(),
                FkPrescriptionId = prescription.PrescriptionId,
                Dosage = item.Dosage,
                DrugName = item.DrugName,
                Duration = item.Duration,
                Frequency = item.Frequency,
                Instructions = item.Instructions,
                IsEssential = item.IsEssential,
                IsPrn = item.IsPrn,
                MealInstructions = item.MealInstructions,
                MedicationType = item.MedicationType,
                Notes = item.Notes,
                Route = item.Route,
                TimeOfDay = item.TimeOfDay,

            };
           
            prescription.AddPrescriptionItem(newItem);
        }

        prescription.CreatePrescription();

        await _work.AddAsync(prescription);

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Prescription to database");
        }

        // Save the domain events to the EventStore
        _eventStoreService.SaveEvents(prescription.Events);

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

        var entry = _work.GetEntityState(entityToUpdate);
        bool hasChanges = entry == EntityState.Modified;

        if (hasChanges == false)
        {
            return false;
        }

        // Trigger domain event for Prescription Updated
        entityToUpdate.UpdatePrescription();

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not update Prescription to database");
        }

        // Save the domain events to the EventStore
        _eventStoreService.SaveEvents(entityToUpdate.Events);

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
        //Trigger delete event
        obj.DeletePrescription();
        
        _work.Remove(obj);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Prescription from database");
        }
        _eventStoreService.SaveEvents(obj.Events);
        return true;
    }
    
}
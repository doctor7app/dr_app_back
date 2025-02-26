using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Interfaces.Services;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;
using Prescriptions.Infrastructure.Services;

namespace Prescriptions.Infrastructure.Implementation.Services;

public class PrescriptionItemService : IPrescriptionItemService
{
    private readonly IRepository<PrescriptionItem, PrescriptionDbContext> _work;
    private readonly IMapper _mapper;
    private readonly IEventStoreService _eventStoreService;

    public PrescriptionItemService(IRepository<PrescriptionItem, PrescriptionDbContext> work,
        IMapper mapper,
        IEventStoreService eventStoreService)
    {
        _work = work;
        _mapper = mapper;
        _eventStoreService = eventStoreService;
    }

    public async Task<PrescriptionItemDto> GetItemByIdAsync(Guid prescriptionId, Guid itemId)
    {
        if (itemId.IsNullOrEmptyGuid() || prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PrescriptionItemId == itemId && x.FkPrescriptionId == prescriptionId);
        return _mapper.Map<PrescriptionItemDto>(obj);
    }

    public async Task<IEnumerable<PrescriptionItemDto>> GetAllItemRelatedToPrescriptionByIdAsync(Guid prescriptionId)
    {
        if (prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetListAsync(x => x.FkPrescriptionId == prescriptionId);

        return _mapper.Map<IEnumerable<PrescriptionItemDto>>(obj);
    }

    public async Task<bool> CreatePrescriptionItem(Guid prescriptionId, PrescriptionItemCreateDto dto)
    {
        if (prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var item = _mapper.Map<PrescriptionItem>(dto);
        await _work.AddAsync(item);
        item.FkPrescriptionId = prescriptionId;
        item.PrescriptionItemId = Guid.NewGuid();

        // Trigger domain event
        item.AddPrescriptionItemEvent();

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Prescription Item to database");
        }
        _eventStoreService.SaveEvents(item.Events);
        //We can add the publish events here
        return true;
    }

    public async Task<bool> UpdateItemAsync(Guid prescriptionId, Guid itemId, Delta<PrescriptionItemUpdateDto> patch)
    {
        if (prescriptionId.IsNullOrEmptyGuid() || itemId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var entityToUpdate = await _work.GetAsync(z => z.PrescriptionItemId == itemId && z.FkPrescriptionId == prescriptionId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {itemId} n'existe pas dans la base de données!");
        }
        var dto = new PrescriptionItemUpdateDto();
        patch.Patch(dto);
        _mapper.Map(dto, entityToUpdate);

        var entry = _work.GetEntityState(entityToUpdate);
        bool hasChanges = entry == EntityState.Modified;

        if (hasChanges == false)
        {
            return false;
        }

        entityToUpdate.UpdatePrescriptionItemEvent();

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not update Prescription Item to database");
        }

        _eventStoreService.SaveEvents(entityToUpdate.Events);
        return true;
    }

    public async Task<bool> DeleteItemAsync(Guid prescriptionId, Guid itemId)
    {
        if (itemId.IsNullOrEmptyGuid() || prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.PrescriptionItemId == itemId && x.FkPrescriptionId == prescriptionId);
        if (obj == null)
        {
            throw new Exception($"L'élement avec l'id {itemId} n'existe pas dans la base de données!");
        }

        obj.RemovePrescriptionItemEvent();
        _work.Remove(obj);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Prescription Item from database");
        }
        // Save the domain events to the EventStore
        _eventStoreService.SaveEvents(obj.Events);

        return true;
    }
}
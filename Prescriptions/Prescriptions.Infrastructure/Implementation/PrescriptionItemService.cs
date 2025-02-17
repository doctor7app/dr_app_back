using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Interfaces;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Implementation;

public class PrescriptionItemService : IPrescriptionItemService
{
    private readonly IRepository<PrescriptionItem, PrescriptionDbContext> _work;
    private readonly IMapper _mapper;

    public PrescriptionItemService(IRepository<PrescriptionItem, PrescriptionDbContext> work,
        IMapper mapper)
    {
        _work = work;
        _mapper = mapper;
    }

    public async Task<PrescriptionItemDto> GetItemByIdAsync(Guid itemId)
    {
        if (itemId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PrescriptionItemId == itemId);
        return _mapper.Map<PrescriptionItemDto>(obj);
    }

    public async Task<IEnumerable<PrescriptionItemDto>> GetAllItemRelatedToPrescriptionByIdAsync(Guid prescriptionId)
    {
        if (prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.FkPrescriptionId == prescriptionId);

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

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Prescription Item to database");
        }

        return true;
    }
    
    public async Task<bool> UpdateItemAsync(Guid id,Guid prescriptionId, Delta<PrescriptionItemUpdateDto> patch)
    {
        if (prescriptionId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var entityToUpdate = await _work.GetAsync(z => z.PrescriptionItemId == id && z.FkPrescriptionId == prescriptionId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {id} n'existe pas dans la base de données!");
        }
        var dto = new PrescriptionItemUpdateDto();
        patch.Patch(dto);

        _mapper.Map(dto, entityToUpdate);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not update Prescription Item to database");
        }
        return true;
    }

    public async Task<bool> DeleteItemAsync(Guid itemId)
    {
        if (itemId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.PrescriptionItemId == itemId);
        if (obj == null)
        {
            throw new Exception($"L'élement avec l'id {itemId} n'existe pas dans la base de données!");
        }
        _work.Remove(obj);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Prescription Item from database");
        }
        return true;
    }
}
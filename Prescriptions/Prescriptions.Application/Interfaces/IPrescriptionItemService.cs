using Microsoft.AspNetCore.OData.Deltas;
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Application.Interfaces;

public interface IPrescriptionItemService
{
    Task<PrescriptionItemDto> GetItemByIdAsync(Guid prescriptionId, Guid itemId);
    Task<IEnumerable<PrescriptionItemDto>> GetAllItemRelatedToPrescriptionByIdAsync(Guid prescriptionId);
    Task<bool> CreatePrescriptionItem(Guid prescriptionId, PrescriptionItemCreateDto dto);
    Task<bool> UpdateItemAsync(Guid prescriptionId, Guid itemId, Delta<PrescriptionItemUpdateDto> patch);
    Task<bool> DeleteItemAsync(Guid prescriptionId, Guid itemId);
}
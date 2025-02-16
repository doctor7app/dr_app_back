using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Application.Interfaces;

public interface IPrescriptionItemService
{
    Task<PrescriptionItemCreateDto> CreatePrescriptionItem(Guid prescriptionId, PrescriptionItemCreateDto dto);
    Task<PrescriptionItemDto> GetItemByIdAsync(Guid itemId);
    Task UpdateItemAsync(PrescriptionItemUpdateDto dto);
    Task DeleteItemAsync(Guid itemId);
}
using Prescriptions.Application.Dtos.Items;

namespace Prescriptions.Application.Interfaces;

public interface IMedicationValidator
{
    Task ValidateMedicationAsync(PrescriptionItemCreateDto dto);
    Task ValidateMedicationUpdateAsync(PrescriptionItemUpdateDto dto);
}
using Prescriptions.Application.Dtos.Events;

namespace Prescriptions.Application.Interfaces.Services;

public interface IPrescriptionHistoryService
{
    Task<IEnumerable<StoredEventDto>> GetPrescriptionHistoryAsync(Guid prescriptionId);
    Task<IEnumerable<StoredEventDto>> GetPrescriptionItemHistoryAsync(Guid prescriptionItemId);
}
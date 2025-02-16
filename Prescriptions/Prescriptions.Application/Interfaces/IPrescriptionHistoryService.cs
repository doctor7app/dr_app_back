using Prescriptions.Application.Dtos.Events;

namespace Prescriptions.Application.Interfaces;

public interface IPrescriptionHistoryService
{
    Task RevertPrescriptionToVersionAsync(Guid prescriptionId, Guid eventId);
    Task<List<PrescriptionEventDto>> GetPrescriptionHistoryAsync(Guid prescriptionId);
}
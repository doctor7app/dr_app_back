using Prescriptions.Application.Dtos.Events;

namespace Prescriptions.Application.Interfaces;

public interface IPrescriptionEventService
{
    Task<List<PrescriptionEventDto>> GetEventsByPrescriptionAsync(Guid prescriptionId);
    Task<PrescriptionEventDetailDto> GetEventDetailsAsync(Guid eventId);
}
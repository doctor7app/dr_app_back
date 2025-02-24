using Common.Enums.Prescriptions;
using Prescriptions.Domain.Abstract;

namespace Prescriptions.Domain.Events;

public class PrescriptionUpdatedEvent :PrescriptionEvent
{
    public Guid PrescriptionId { get; set; }
    public string? Notes { get; set; }
    public PrescriptionStatus Status { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public PrescriptionUpdatedEvent(Guid prescriptionId,
        string? notes,
        PrescriptionStatus status,
        DateTime? expirationDate)
    {
        PrescriptionId = prescriptionId;
        Notes = notes;
        Status = status;
        ExpirationDate = expirationDate;
    }
}
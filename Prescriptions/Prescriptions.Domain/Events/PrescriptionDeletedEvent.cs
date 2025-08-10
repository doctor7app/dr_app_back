using Prescriptions.Domain.Abstract;

namespace Prescriptions.Domain.Events;

public class PrescriptionDeletedEvent : PrescriptionEvent
{
    public Guid PrescriptionId { get; set; }
    public PrescriptionDeletedEvent(Guid prescriptionId)
    {
        PrescriptionId = prescriptionId;
    }
}
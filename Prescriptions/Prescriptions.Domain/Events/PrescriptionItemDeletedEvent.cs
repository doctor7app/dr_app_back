using Prescriptions.Domain.Abstract;

namespace Prescriptions.Domain.Events;

public class PrescriptionItemDeletedEvent : PrescriptionEvent
{
    public Guid PrescriptionItemId { get; set; }
    public PrescriptionItemDeletedEvent(Guid prescriptionItemId)
    {
        PrescriptionItemId = prescriptionItemId;
    }
}
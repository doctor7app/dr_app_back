using Prescriptions.Domain.Abstract;

namespace Prescriptions.Domain.Events;

public class PrescriptionItemCreatedEvent : PrescriptionEvent
{
    public Guid PrescriptionItemId { get; set; }
    public string DrugName { get; set; }
    public string Dosage { get; set; }

    public PrescriptionItemCreatedEvent(Guid prescriptionItemId, 
        string drugName, 
        string dosage)
    {
        PrescriptionItemId = prescriptionItemId;
        DrugName = drugName;
        Dosage = dosage;
    }
}
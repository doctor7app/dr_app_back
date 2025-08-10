using Prescriptions.Domain.Abstract;

namespace Prescriptions.Domain.Events;

public class PrescriptionCreatedEvent : PrescriptionEvent
{
    public Guid PrescriptionId { get; set; }
    public DateTime IssuedAt { get; set; }
    public Guid FkPatientId { get; set; }
    public Guid FkDoctorId { get; set; }


    public PrescriptionCreatedEvent(Guid prescriptionId,DateTime issuedAt,Guid fkPatientId,Guid fkDoctorId)
    {
        PrescriptionId = prescriptionId;
        IssuedAt = issuedAt;
        FkPatientId = fkPatientId;
        FkDoctorId = fkDoctorId;
    }
    
}
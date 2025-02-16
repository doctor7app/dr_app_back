using Common.Enums.Patients;

namespace Contracts.Messages.Dmes;

public class DmeUpdatedEvent
{
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public string AdditionalInformations { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public PatientState State { get; set; }
}
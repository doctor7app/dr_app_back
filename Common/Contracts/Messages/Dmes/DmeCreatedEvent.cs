using Common.Enums;

namespace Contracts.Messages.Dmes;

public class DmeCreatedEvent
{
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public string AdditionalInformations { get; set; }

    public string PatientName { get; set; }
    //Must be unique in the application, one patient can have one dme only.
    public Guid PatientId { get; set; }
    //Can be removed.
    public string DoctorName { get; set; }
    public Guid DoctorId { get; set; }

    public DateTime Created { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid LastModifiedById { get; set; }

    public PatientState State { get; set; }
}
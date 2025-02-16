using Common.Enums;

namespace Contracts.Messages.Patients;

public class PatientUpdatedEvent
{
    public Guid Id { get; set; }
    public string SocialNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string HomeNumber { get; set; }
}
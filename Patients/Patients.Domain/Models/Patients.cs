using Common.Classes;
using Common.Enums;

namespace Patients.Domain.Models;

public class Patient : AuditableEntity
{
    public Guid PatientId { get; set; }
    public string SocialSecurityNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime DeathDate { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string HomeNumber { get; set; }

    public ICollection<Adresse> Adresses { get; set; }
    public ICollection<Contact> Contacts { get; set; }
    public ICollection<MedicalInformation> MedicalInformations { get; set; }
}
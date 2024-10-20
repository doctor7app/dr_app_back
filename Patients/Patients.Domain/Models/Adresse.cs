using Common.Classes;
using Common.Enums;


namespace Patients.Domain.Models;

public class Adresse : AuditableEntity
{
    public Guid AdresseId { get; set; }
    public string Country { get; set; }
    public string Provence { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public string AdditionalInformation { get; set; }

    public AdresseType Type { get; set; }
    
    
    public Guid FkIdPatient { get; set; }
    public Patient Patient { get; set; }
}
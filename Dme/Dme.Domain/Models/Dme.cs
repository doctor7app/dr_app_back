using Common.Classes;
using Common.Enums;

namespace Dme.Domain.Models;

/// <summary>
/// This class represent the "Dossier Medical Electronique" table.
/// </summary>
public class Dme : AuditableEntity
{
    public Guid DmeId { get; set; }
    public string Notes { get; set; }
    public string AdditionalInformations { get; set; }
    public Guid FkIdPatient { get; set; }
    public Guid FkIdDoctor { get; set; }
    public PatientState State { get; set; }

    public ICollection<Consultations> Consultations { get; set; }
}
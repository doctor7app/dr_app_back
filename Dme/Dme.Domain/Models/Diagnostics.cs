namespace Dme.Domain.Models;

public class Diagnostics
{
    public Guid DiagnosticId { get; set; }
    /// <summary>
    /// ex: radiologie, laboratoire
    /// </summary>
    public string TypeDiagnostic { get; set; }
    public string Description { get; set; }
    public string Results { get; set; }
    public string Comments { get; set; }

    public Guid FkIdConsultation { get; set; }
    public Consultations Consultation { get; set; }
}
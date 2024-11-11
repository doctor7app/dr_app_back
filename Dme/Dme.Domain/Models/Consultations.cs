using Common.Classes;
using Common.Enums;

namespace Dme.Domain.Models;

/// <summary>
/// This class represent "Consultation" table.
/// </summary>
public class Consultations : AuditableEntity
{
    public Guid ConsultationId { get; set; }
    public string ReasonOfVisit { get; set; }
    public string Symptoms { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public string PressureArterial { get; set; }
    public decimal Temperature { get; set; }
    public int CardiacFrequency { get; set; }
    public decimal SaturationOxygen { get; set; }
    public DateTime ConsultationDate { get; set; }
    public DateTime? NextConsultationDate { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationState State { get; set; }

    public Dme Dme { get; set; }
    public Guid FkIdDme { get; set; }

    public ICollection<Diagnostics> Diagnostics { get; set; }
    public ICollection<Treatments> Treatments { get; set; }
}
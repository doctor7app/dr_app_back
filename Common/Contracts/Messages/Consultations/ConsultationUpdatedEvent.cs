using Common.Enums;

namespace Contracts.Messages.Consultations;

public class ConsultationUpdatedEvent
{
    public Guid Id { get; set; }
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
}
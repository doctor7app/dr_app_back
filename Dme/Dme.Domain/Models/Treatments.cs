namespace Dme.Domain.Models;

public class Treatments
{
    public Guid TreatmentsId { get; set; }
    public string Medicament { get; set; }
    public string Dose { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }

    public Guid FkIdConsultation { get; set; }
    public Consultations Consultation { get; set; }
}
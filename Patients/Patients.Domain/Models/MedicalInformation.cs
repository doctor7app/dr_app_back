using Common.Enums;

namespace Patients.Domain.Models;

public class MedicalInformation
{
    public Guid MedicalInformationId { get; set; }
    public MedicalInformationType Type { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }

    public Guid FkIdPatient { get; set; }
    public Patient Patient { get; set; }
}
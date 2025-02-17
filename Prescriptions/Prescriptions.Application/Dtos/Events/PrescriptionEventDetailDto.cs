using System.Text.Json;

namespace Prescriptions.Application.Dtos.Events;

public class PrescriptionEventDetailDto : PrescriptionEventDto
{
    public object EventData { get; set; }
}
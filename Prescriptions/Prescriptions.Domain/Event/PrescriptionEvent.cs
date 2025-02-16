using System.Text.Json;
using Common.Enums.Prescriptions;

namespace Prescriptions.Domain.Event;

public class PrescriptionEvent
{
    public Guid PrescriptionEventId { get; set; }
    
    public PrescriptionEventType EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DoctorId { get; set; }
    public string EventDataJson { get; set; }

    public Guid FkPrescriptionId { get; set; }

    // Constructeur privé pour contrôler la création
    private PrescriptionEvent() { }

    // Factory Method (pattern de création)
    public static PrescriptionEvent Create(
        Guid prescriptionId,
        PrescriptionEventType eventType,
        Guid doctorId,
        object eventData)
    {
        return new PrescriptionEvent
        {
            PrescriptionEventId = Guid.NewGuid(),
            FkPrescriptionId = prescriptionId,
            EventType = eventType,
            DoctorId = doctorId,
            EventDataJson = JsonSerializer.Serialize(eventData),
            Timestamp = DateTime.UtcNow
        };
    }
}
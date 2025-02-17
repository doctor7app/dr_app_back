using System.Text.Json;
using Common.Enums.Prescriptions;
using Prescriptions.Domain.Models;

namespace Prescriptions.Domain.Event;

public class PrescriptionEvent
{
    public Guid PrescriptionEventId { get; set; }
    public PrescriptionEventType EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventDataJson { get; set; }

    public Guid FkPrescriptionId { get; set; }
    public Guid FkDoctorId { get; set; }

    public Prescription Prescription { get; set; }
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
            FkDoctorId = doctorId,
            EventDataJson = JsonSerializer.Serialize(eventData),
            Timestamp = DateTime.UtcNow
        };
    }
}
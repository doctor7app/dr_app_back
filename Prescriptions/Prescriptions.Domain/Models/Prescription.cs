using Common.Enums;
using Common.Enums.Prescriptions;
using Prescriptions.Domain.Common;
using Prescriptions.Domain.Event;

namespace Prescriptions.Domain.Models;

public class Prescription : IAggregateRoot
{
    public Guid PrescriptionId { get; set; }
    public DateTime IssuedAt { get; set; }
    public string? Notes { get; set; }
    public PrescriptionStatus Status { get; set; }
    public ConsultationType ConsultationType { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public Guid FkPatientId { get; set; }
    public Guid FkConsultationId { get; set; }
    public Guid FkDoctorId { get; set; }
    private readonly List<PrescriptionItem> _items = new();
    public IReadOnlyCollection<PrescriptionItem> Items => _items.AsReadOnly();

    // === Gestion des événements métier ===
    private readonly List<PrescriptionEvent> _domainEvents = new();
    public IReadOnlyCollection<PrescriptionEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Constructeur (pour création initiale)
    public Prescription(
        Guid id,
        Guid patientId,
        Guid consultationId,
        Guid doctorId,
        ConsultationType consultationType)
    {
        PrescriptionId = id;
        FkPatientId = patientId;
        FkConsultationId = consultationId;
        FkDoctorId = doctorId;
        ConsultationType = consultationType;
        IssuedAt = DateTime.UtcNow;
        Status = PrescriptionStatus.Draft;

        // Événement de création
        _domainEvents.Add(PrescriptionEvent.Create(
            PrescriptionId,
            PrescriptionEventType.PrescriptionCreated,
            FkDoctorId,
            new { ConsultationType = ConsultationType }));
    }

    // === Méthodes métier (modifications) ===

    public void AddMedication(
        string drugName,
        string dosage,
        string frequency,
        string duration,
        Guid doctorId)
    {
        var item = new PrescriptionItem
        {
            PrescriptionItemId = Guid.NewGuid(),
            DrugName = drugName,
            Dosage = dosage,
            Frequency = frequency,
            Duration = duration
        };

        _items.Add(item);

        // Événement
        _domainEvents.Add(PrescriptionEvent.Create(
            PrescriptionId,
            PrescriptionEventType.MedicationAdded,
            doctorId,
            new { DrugName = drugName, Dosage = dosage }));
    }

    public void UpdateMedicationDosage(
        Guid medicationId,
        string newDosage,
        Guid doctorId)
    {
        var item = _items.FirstOrDefault(i => i.PrescriptionItemId == medicationId);
        if (item == null) throw new ArgumentException("Médicament non trouvé.");

        var oldDosage = item.Dosage;
        item.Dosage = newDosage;

        // Événement
        _domainEvents.Add(PrescriptionEvent.Create(
            PrescriptionId,
            PrescriptionEventType.MedicationDosageUpdated,
            doctorId,
            new { MedicationId = medicationId, OldDosage = oldDosage, NewDosage = newDosage }));
    }

    public void RemoveMedication(Guid medicationId, Guid doctorId)
    {
        var item = _items.FirstOrDefault(i => i.PrescriptionItemId == medicationId);
        if (item == null) return;

        _items.Remove(item);

        // Événement
        _domainEvents.Add(PrescriptionEvent.Create(
            PrescriptionId,
            PrescriptionEventType.MedicationRemoved,
            doctorId,
            new { MedicationId = medicationId }));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
using Common.Enums;
using Common.Enums.Prescriptions;
using Prescriptions.Domain.Abstract;
using Prescriptions.Domain.Events;

namespace Prescriptions.Domain.Models;

public class Prescription : AggregateRoot
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
    public IEnumerable<PrescriptionItem> Items => _items.AsReadOnly();

    public void CreatePrescription()
    {
        AddEvent(new PrescriptionCreatedEvent(PrescriptionId, IssuedAt, FkPatientId, FkDoctorId));
    }

    public void UpdatePrescription()
    {
        AddEvent(new PrescriptionUpdatedEvent(PrescriptionId, Notes, Status, ExpirationDate));
    }

    public void DeletePrescription()
    {
        AddEvent(new PrescriptionDeletedEvent(PrescriptionId));
    }

    public void AddPrescriptionItem(PrescriptionItem item)
    {
        _items.Add(item);
        AddEvent(new PrescriptionItemCreatedEvent(item.PrescriptionItemId, item.DrugName, item.Dosage));
    }

    public void UpdatePrescriptionItem(PrescriptionItem item)
    {
        var existingItem = _items.FirstOrDefault(x => x.PrescriptionItemId == item.PrescriptionItemId);
        if (existingItem != null)
        {
            existingItem.Update(item);
            AddEvent(new PrescriptionItemUpdatedEvent(item.PrescriptionItemId, item.DrugName, item.Dosage));
        }
    }

    public void RemovePrescriptionItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.PrescriptionItemId == itemId);
        if (item != null)
        {
            _items.Remove(item);
            AddEvent(new PrescriptionItemDeletedEvent(itemId));
        }
    }
}
using Common.Enums.Prescriptions;
using Prescriptions.Domain.Events;

namespace Prescriptions.UnitTest.Domain;

public class PrescriptionEventsTests
{
    [Fact]
    public void Constructor_PrescriptionCreated_SetsCorrectProperties()
    {
        // Arrange
        var prescriptionId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var issuedAt = DateTime.UtcNow;

        // Act
        var @event = new PrescriptionCreatedEvent(prescriptionId, issuedAt, patientId, doctorId);

        // Assert
        Assert.Equal(prescriptionId, @event.PrescriptionId);
        Assert.Equal(patientId, @event.FkPatientId);
        Assert.Equal(doctorId, @event.FkDoctorId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }

    [Fact]
    public void Constructor_PrescriptionUpdatedEvent_SetsCorrectProperties()
    {
        // Arrange
        var prescriptionId = Guid.NewGuid();
        const string notes = "Important notes";
        var expirationDate = DateTime.UtcNow.AddDays(30);

        // Act
        var @event = new PrescriptionUpdatedEvent(
            prescriptionId,
            notes,
            PrescriptionStatus.Validated,
            expirationDate);

        // Assert
        Assert.Equal(notes, @event.Notes);
        Assert.Equal(PrescriptionStatus.Validated, @event.Status);
        Assert.Equal(expirationDate, @event.ExpirationDate);
    }

    [Fact]
    public void Constructor_ItemUpdated_SetsCorrectValues()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        const string drugName = "Updated Drug";
        const string dosage = "25mg";

        // Act
        var @event = new PrescriptionItemUpdatedEvent(itemId, drugName, dosage);

        // Assert
        Assert.Equal(itemId, @event.PrescriptionItemId);
        Assert.Equal(drugName, @event.DrugName);
        Assert.Equal(dosage, @event.Dosage);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
    }
}
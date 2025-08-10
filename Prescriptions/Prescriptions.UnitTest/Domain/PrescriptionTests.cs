using Common.Enums.Prescriptions;
using Prescriptions.Domain.Events;
using Prescriptions.Domain.Models;

namespace Prescriptions.UnitTest.Domain;

public class PrescriptionTests
{
    [Fact]
    public void Constructor_InitializesProperly()
    {
        // Arrange & Act
        var prescription = new Prescription
        {
            PrescriptionId = Guid.NewGuid(),
            IssuedAt = DateTime.UtcNow,
            FkPatientId = Guid.NewGuid(),
            FkConsultationId = Guid.NewGuid(),
            FkDoctorId = Guid.NewGuid()
        };

        // Assert
        Assert.NotNull(prescription.Items);
        Assert.Empty(prescription.Events);
    }

    [Fact]
    public void CreatePrescription_AddsCreatedEvent()
    {
        // Arrange
        var prescription = new Prescription
        {
            PrescriptionId = Guid.NewGuid(),
            IssuedAt = DateTime.UtcNow,
            FkPatientId = Guid.NewGuid(),
            FkDoctorId = Guid.NewGuid()
        };

        // Act
        prescription.CreatePrescription();

        // Assert
        var @event = prescription.Events.Single() as PrescriptionCreatedEvent;
        Assert.NotNull(@event);
        Assert.Equal(prescription.PrescriptionId, @event.PrescriptionId);
    }

    [Fact]
    public void AddPrescriptionItem_AddsItemAndEvent()
    {
        // Arrange
        var prescription = new Prescription();
        var item = new PrescriptionItem { PrescriptionItemId = Guid.NewGuid() };

        // Act
        prescription.AddPrescriptionItem(item);

        // Assert
        Assert.Single(prescription.Items);
        Assert.IsType<PrescriptionItemCreatedEvent>(prescription.Events.Last());
    }

    [Fact]
    public void UpdatePrescription_AddsUpdatedEvent()
    {
        // Arrange
        var prescription = new Prescription { PrescriptionId = Guid.NewGuid() };
        prescription.Notes = "Updated notes";
        prescription.Status = PrescriptionStatus.Validated;

        // Act
        prescription.UpdatePrescription();

        // Assert
        var @event = prescription.Events.Single() as PrescriptionUpdatedEvent;
        Assert.Equal("Updated notes", @event?.Notes);
        Assert.Equal(PrescriptionStatus.Validated, @event?.Status);
    }

    [Fact]
    public void RemovePrescriptionItem_RemovesItemAndAddsEvent()
    {
        // Arrange
        var prescription = new Prescription();
        var item = new PrescriptionItem { PrescriptionItemId = Guid.NewGuid() };
        prescription.AddPrescriptionItem(item);

        // Act
        prescription.RemovePrescriptionItem(item.PrescriptionItemId);

        // Assert
        Assert.Empty(prescription.Items);
        Assert.IsType<PrescriptionItemDeletedEvent>(prescription.Events.Last());
    }
}
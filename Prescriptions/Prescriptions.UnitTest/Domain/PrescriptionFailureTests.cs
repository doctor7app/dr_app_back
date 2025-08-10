using Prescriptions.Domain.Models;

namespace Prescriptions.UnitTest.Domain;

public class PrescriptionFailureTests
{
    [Fact]
    public void UpdateNonExistentItem_DoesNothing()
    {
        // Arrange
        var prescription = new Prescription();
        var nonExistentItem = new PrescriptionItem { PrescriptionItemId = Guid.NewGuid() };

        // Act
        prescription.UpdatePrescriptionItem(nonExistentItem);

        // Assert
        Assert.Empty(prescription.Items);
        Assert.Empty(prescription.Events);
    }

    [Fact]
    public void RemoveNonExistentItem_DoesNothing()
    {
        // Arrange
        var prescription = new Prescription();
        var existingItem = new PrescriptionItem { PrescriptionItemId = Guid.NewGuid() };
        prescription.AddPrescriptionItem(existingItem);

        // Act
        prescription.RemovePrescriptionItem(Guid.NewGuid());

        // Assert
        Assert.Single(prescription.Items);
        Assert.Single(prescription.Events);
    }
}
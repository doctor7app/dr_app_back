using Common.Extension.Common;
using Dme.Domain.Models;

namespace Dme.UnitTest.Models;

public class TreatmentsTests
{
    [Fact]
    public void Treatments_Constructor_SetsProperties()
    {
        // Arrange
        var treatment = new Treatments
        {
            TreatmentsId = Guid.NewGuid(),
            Medicament = "Aspirin",
            Dose = "500mg",
            Frequency = "Once a day",
            Duration = "7 days",
            Instructions = "Take after meals"
        };

        // Act

        // Assert
        Assert.NotNull(treatment);
        Assert.Equal("Aspirin", treatment.Medicament);
        Assert.Equal("500mg", treatment.Dose);
    }

    [Fact]
    public void Treatments_Equality_ChecksEqualObjects()
    {
        // Arrange
        var treatment1 = new Treatments { TreatmentsId = Guid.NewGuid(), Medicament = "Aspirin" };
        var treatment2 = new Treatments { TreatmentsId = treatment1.TreatmentsId, Medicament = "Aspirin" };

        // Act

        // Assert
        Assert.True(treatment1.AreEqual(treatment2)); // Assuming AreEqual is an extension method
    }
}
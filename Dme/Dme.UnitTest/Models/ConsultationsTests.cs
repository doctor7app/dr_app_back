using Common.Enums;
using Common.Extension.Common;
using Dme.Domain.Models;

namespace Dme.UnitTest.Models;

public class ConsultationsTests
{
    [Fact]
    public void Consultations_Constructor_SetsProperties()
    {
        // Arrange
        var consultation = new Consultations
        {
            ConsultationId = Guid.NewGuid(),
            ReasonOfVisit = "Checkup",
            Symptoms = "None",
            Weight = 70.5m,
            Height = 175.0m,
            ConsultationDate = DateTime.Now,
            Type = ConsultationType.Emergency,
            State = ConsultationState.Completed
        };

        // Act

        // Assert
        Assert.NotNull(consultation);
        Assert.Equal("Checkup", consultation.ReasonOfVisit);
        Assert.Equal("None", consultation.Symptoms);
        Assert.Equal(70.5m, consultation.Weight);
    }

    [Fact]
    public void Consultations_Equality_ChecksEqualObjects()
    {
        // Arrange
        var consultation1 = new Consultations { ConsultationId = Guid.NewGuid(), ReasonOfVisit = "Checkup" };
        var consultation2 = new Consultations { ConsultationId = consultation1.ConsultationId, ReasonOfVisit = "Checkup" };

        // Act

        // Assert
        Assert.True(consultation1.AreEqual(consultation2)); // Assuming AreEqual is an extension method
    }
}
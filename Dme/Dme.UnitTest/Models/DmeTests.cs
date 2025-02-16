using Common.Extension.Common;

namespace Dme.UnitTest.Models;

public class DmeTests
{
    [Fact]
    public void Dme_Constructor_SetsProperties()
    {
        // Arrange
        var dme = new Domain.Models.Dme
        {
            DmeId = Guid.NewGuid(),
            Notes = "Initial notes",
            AdditionalInformations = "Some additional info",
            FkIdPatient = Guid.NewGuid(),
            FkIdDoctor = Guid.NewGuid()
        };

        // Act

        // Assert
        Assert.NotNull(dme);
        Assert.Equal("Initial notes", dme.Notes);
        Assert.Equal("Some additional info", dme.AdditionalInformations);
    }

    [Fact]
    public void Dme_Equality_ChecksEqualObjects()
    {
        // Arrange
        var dme1 = new Domain.Models.Dme { DmeId = Guid.NewGuid(), Notes = "Initial notes" };
        var dme2 = new Domain.Models.Dme { DmeId = dme1.DmeId, Notes = "Initial notes" };

        // Act

        // Assert
        Assert.True(dme1.AreEqual(dme2)); // Assuming AreEqual is an extension method
    }
}
using Common.Enums.Patients;
using Common.Extension.Common;
using Patients.Domain.Models;

namespace Patients.UnitTest.Models;

public class AdresseTests
{
    [Fact]
    public void Adresse_Constructor_SetsProperties()
    {
        // Arrange
        var adresse = new Adresse
        {
            AdresseId = Guid.NewGuid(),
            Country = "Country",
            Provence = "Provence",
            City = "City",
            PostalCode = "12345",
            Street = "123 Main St",
            AdditionalInformation = "Near park",
            FkIdPatient = Guid.NewGuid(),
            Type = AdresseType.Home
        };

        // Act

        // Assert
        Assert.NotNull(adresse);
        Assert.Equal("Country", adresse.Country);
        Assert.Equal("Provence", adresse.Provence);
        Assert.Equal("City", adresse.City);
        Assert.Equal("12345", adresse.PostalCode);
        Assert.Equal("123 Main St", adresse.Street);
        Assert.Equal("Near park", adresse.AdditionalInformation);
    }

    [Fact]
    public void Adresse_Equality_ChecksEqualObjects()
    {
        // Arrange
        var adresse1 = new Adresse { AdresseId = Guid.NewGuid(), Country = "Country" };
        var adresse2 = new Adresse { AdresseId = adresse1.AdresseId, Country = "Country" };

        // Act

        // Assert
        Assert.True(adresse1.AreEqual(adresse2));
    }
}
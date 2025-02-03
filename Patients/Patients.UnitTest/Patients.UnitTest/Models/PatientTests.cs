using Common.Extension.Common;
using Patients.Domain.Models;

namespace Patients.UnitTest.Models;

public class PatientTests
{
    [Fact]
    public void Patient_Constructor_SetsProperties()
    {
        // Arrange
        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            SocialSecurityNumber = "123-45-6789",
            FirstName = "Jane",
            LastName = "Doe",
            BirthDate = new DateTime(1990, 1, 1),
            Adresses = new List<Adresse>(),
            Contacts = new List<Contact>(),
            MedicalInformations = new List<MedicalInformation>()
        };

        // Act

        // Assert
        Assert.NotNull(patient);
        Assert.Equal("Jane", patient.FirstName);
        Assert.Equal("Doe", patient.LastName);
        Assert.Equal(0, patient.Adresses.Count);
    }

    [Fact]
    public void Patient_Equality_ChecksEqualObjects()
    {
        // Arrange
        var patient1 = new Patient { PatientId = Guid.NewGuid(), FirstName = "Jane" };
        var patient2 = new Patient { PatientId = patient1.PatientId, FirstName = "Jane" };

        // Act

        // Assert
        Assert.True(patient1.AreEqual(patient2));
    }
}
using Common.Enums;
using Common.Extension.Common;
using Patients.Domain.Models;

namespace Patients.UnitTest.Models;

public class MedicalInformationTests
{
    [Fact]
    public void MedicalInformation_Constructor_SetsProperties()
    {
        // Arrange
        var medicalInfo = new MedicalInformation
        {
            MedicalInformationId = Guid.NewGuid(),
            Type = MedicalInformationType.AllergiesAndReactions,
            Name = "Peanut Allergy",
            Note = "Severe reaction",
            FkIdPatient = Guid.NewGuid()
        };

        // Act

        // Assert
        Assert.NotNull(medicalInfo);
        Assert.Equal("Peanut Allergy", medicalInfo.Name);
        Assert.Equal("Severe reaction", medicalInfo.Note);
        Assert.Equal(1, (double)medicalInfo.Type);
    }

    [Fact]
    public void MedicalInformation_Equality_ChecksEqualObjects()
    {
        // Arrange
        var medicalInfo1 = new MedicalInformation { MedicalInformationId = Guid.NewGuid(), Name = "Peanut Allergy" };
        var medicalInfo2 = new MedicalInformation { MedicalInformationId = medicalInfo1.MedicalInformationId, Name = "Peanut Allergy" };

        // Act

        // Assert
        Assert.True(medicalInfo1.AreEqual(medicalInfo2));
    }
}
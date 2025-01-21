using Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Api.Controllers;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;

namespace Patients.UnitTest;

public class PatientControllerTest
{

    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IContactService> _contractServiceMock;
    private readonly Mock<IAdresseService> _adresseServiceMock;
    private readonly Mock<IMedicalInfoService> _medicalInfoServiceMock;

    public PatientControllerTest()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _contractServiceMock = new Mock<IContactService>();
        _adresseServiceMock = new Mock<IAdresseService>();
        _medicalInfoServiceMock = new Mock<IMedicalInfoService>();

        
    }

    [Fact]
    public async void GetPatient_ReturnsPatient_WhenPatientExists()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var expectedPatient = new PatientDto
        {
            Id = patientId,
            FirstName = "Joe",
            LastName = "Lancelot",
            BirthDate = new DateTime(1950,12,1),
            DeathDate = new DateTime(),
            Email = "Joe.Lancelot@gmail.com",
            Gender = Gender.Male,
            HomeNumber = "216 56 015 556"
        };

        _patientServiceMock.Setup(x => x.Get(patientId)).ReturnsAsync(expectedPatient);

        var patientController = new PatientsController(_patientServiceMock.Object,
            _contractServiceMock.Object,
            _adresseServiceMock.Object,
            _medicalInfoServiceMock.Object);

        //Act 
        var result = await patientController.Get(patientId);
        
        //Assert
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(expectedPatient, okResult.Value);
        Assert.Equal(expectedPatient.GetType(),okResult.Value?.GetType());

    }
}
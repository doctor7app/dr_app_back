using Common.Enums.Patients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Api.Controllers;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;

namespace Patients.UnitTest.Controllers;

public class PatientControllerTests
{

    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IContactService> _contractServiceMock;
    private readonly Mock<IAdresseService> _adresseServiceMock;
    private readonly Mock<IMedicalInfoService> _medicalInfoServiceMock;
    private readonly PatientsController _patientController;

    public PatientControllerTests()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _contractServiceMock = new Mock<IContactService>();
        _adresseServiceMock = new Mock<IAdresseService>();
        _medicalInfoServiceMock = new Mock<IMedicalInfoService>();

        _patientController = new PatientsController(_patientServiceMock.Object,
            _contractServiceMock.Object,
            _adresseServiceMock.Object,
            _medicalInfoServiceMock.Object);
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
            BirthDate = new DateTime(1950, 12, 1),
            DeathDate = new DateTime(),
            Email = "Joe.Lancelot@gmail.com",
            Gender = Gender.Male,
            HomeNumber = "216 56 015 556"
        };

        _patientServiceMock.Setup(x => x.Get(patientId)).ReturnsAsync(expectedPatient);

        //Act 
        var result = await _patientController.Get(patientId);

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(expectedPatient, okResult.Value);
        Assert.Equal(expectedPatient.GetType(), okResult.Value?.GetType());

    }

    [Fact]
    public async void GetPatient_ReturnsEmpty_WhenPatientDoesNotExists()
    {
        //Arrange
        var patientId = Guid.NewGuid();

        _patientServiceMock.Setup(x => x.Get(patientId)).ReturnsAsync(null!);

        //Act 
        var result = await _patientController.Get(patientId);

        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(okResult.Value);
    }

    [Fact]
    public async void GetAll_ReturnsOkResult_WithListOfPatients()
    {
        //Arrange
        var patients = new List<PatientDto>
        {
            new(){Id = Guid.NewGuid(),FirstName = "Joe",LastName = "LanceLot", BirthDate = new DateTime(1952,1,1)},
            new(){Id = Guid.NewGuid(),FirstName = "LanceLot",LastName = "Joe", BirthDate = new DateTime(1954,1,1)}
        };
        _patientServiceMock.Setup(a => a.Get()).ReturnsAsync(patients);

        //Act
        var result = await _patientController.Get();

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPatients = Assert.IsAssignableFrom<List<PatientDto>>(okResult.Value);
        Assert.Equal(2, returnedPatients.Count);
        Assert.Equal(patients, returnedPatients);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithEmptyList_WhenNoPatientsFound()
    {
        // Arrange
        var patients = new List<PatientDto>(); 
        _patientServiceMock.Setup(x => x.Get()).ReturnsAsync(patients);

        // Act
        var result = await _patientController.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPatients = Assert.IsAssignableFrom<List<PatientDto>>(okResult.Value);
        Assert.Empty(returnedPatients); 
    }

    [Fact]
    public async void CreatePatient_ReturnTrue_WhenModalIsValid()
    {
        //Arrange 
        var patientToCreate = new PatientCreateDto
        {
            BirthDate = new DateTime(1950, 12, 1),
            DeathDate = null,
            FirstName = "Joe",
            LastName = "LanceLot",
            Gender = Gender.Male,
            Email = "Joe.LanceLot@gmail.com",
            HomeNumber = "98900900",
            MiddleName = "",
            PhoneNumber = "98900900",
            SocialNumber = "012457896"
        };

        _patientServiceMock.Setup(s => s.Create(patientToCreate)).ReturnsAsync(true);

        // Act
        var result = await _patientController.Post(patientToCreate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void CreatePatient_ReturnFalse_WhenIsModalNotValid()
    {
        //Arrange 
        var patientToCreate = new PatientCreateDto
        {
            BirthDate = new DateTime(1950, 12, 1),
            DeathDate = null,

            Email = "Joe.LanceLot@gmail.com",
            HomeNumber = "98900900",
            MiddleName = "",
            PhoneNumber = "98900900",
            SocialNumber = "012457896"
        };

        _patientController.ModelState.AddModelError("FirstName", "First Name is Required");
        _patientController.ModelState.AddModelError("LastName", "Last Name is Required");

        _patientServiceMock.Setup(s => s.Create(patientToCreate)).ReturnsAsync(false);

        //Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.Post(patientToCreate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdatePatient_ReturnTrue_WhenPatientExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var patientToUpdate = new PatientPatchDto
        {
            LastName = "Lancelot",
            FirstName = "Jeo",
            Gender = Gender.Male,
            Email = "Jeo@gmail.com",
            HomeNumber = "72700700",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            PhoneNumber = "98900900",
            SocialNumber = "123456789"
        };

        _patientServiceMock.Setup(x => x.Patch(patientId, patientToUpdate)).ReturnsAsync(true);

        //Act

        var result = await _patientController.Patch(patientId, patientToUpdate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void UpdatePatient_ReturnTrue_WhenModalIsNotValid()
    {
        var patientId = Guid.NewGuid();
        var patientToUpdate = new PatientPatchDto
        {
            Gender = Gender.Male,
            Email = "Jeo@gmail.com",
            HomeNumber = "72700700",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            PhoneNumber = "98900900",
            SocialNumber = "123456789"
        };

        _patientServiceMock.Setup(x => x.Patch(patientId, patientToUpdate)).ReturnsAsync(false);
        _patientController.ModelState.AddModelError("FirstName", "First Name is Required");
        _patientController.ModelState.AddModelError("LastName", "Last Name is Required");

        //Act & Assert

        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.Patch(patientId, patientToUpdate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdatePatient_ReturnException_WhenPatientDoesNotExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var patientToUpdate = new PatientPatchDto
        {
            LastName = "Lancelot",
            FirstName = "Jeo",
            Gender = Gender.Male,
            Email = "Jeo@gmail.com",
            HomeNumber = "72700700",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            PhoneNumber = "98900900",
            SocialNumber = "123456789"
        };
        _patientServiceMock.Setup(x => x.Patch(patientId, patientToUpdate))
            .ReturnsAsync(new Exception($"L'élement avec l'id {patientId} n'existe pas dans la base de données!"));

        // Act
        var result = await _patientController.Patch(patientId, patientToUpdate);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {patientId} n'existe pas dans la base de données!", objectResult.Value);
    }

    [Fact]
    public async void DeletePatient_ReturnTrue_WhenPatientExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();

        _patientServiceMock.Setup(a => a.Delete(patientId)).ReturnsAsync(true);

        //Act
        var result = await _patientController.Delete(patientId);

        //Assert

        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, checkResult.StatusCode);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void DeletePatient_ReturnTrue_WhenPatientDoesNotExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        _patientServiceMock.Setup(x => x.Delete(patientId))
            .ReturnsAsync(new Exception($"L'élement avec l'id {patientId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.Delete(patientId);

        //Assert

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {patientId} n'existe pas dans la base de données!", objectResult.Value);
    }
}
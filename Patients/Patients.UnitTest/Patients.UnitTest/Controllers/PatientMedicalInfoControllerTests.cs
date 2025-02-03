using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Api.Controllers;
using Patients.Application.DTOs.MedicalInfo;
using Patients.Application.Interfaces;

namespace Patients.UnitTest.Controllers;

public class PatientMedicalInfoControllerTests
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IContactService> _contactServiceMock;
    private readonly Mock<IAdresseService> _adresseServiceMock;
    private readonly Mock<IMedicalInfoService> _medicalInfoServiceMock;
    private readonly PatientsController _patientController;

    public PatientMedicalInfoControllerTests()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _contactServiceMock = new Mock<IContactService>();
        _adresseServiceMock = new Mock<IAdresseService>();
        _medicalInfoServiceMock = new Mock<IMedicalInfoService>();

        _patientController = new PatientsController(_patientServiceMock.Object,
            _contactServiceMock.Object,
            _adresseServiceMock.Object,
            _medicalInfoServiceMock.Object);
    }

    [Fact]
    public async void GetMedicalInfo_ReturnMedicalInfo_WhenMedicalInfoExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var medicalInfoExpected = new MedicalInfoDto
        {
            Id = medicalInfoId,
            Name = "Weight",
            Note = "This is our Note",
            Type = MedicalInformationType.Others
        };
        _medicalInfoServiceMock.Setup(a => a.GetMedicalInfo(patientId, medicalInfoId)).ReturnsAsync(medicalInfoExpected);

        //Act
        var result = await _patientController.GetMedical(patientId, medicalInfoId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(medicalInfoExpected, okResult.Value);
        Assert.Equal(medicalInfoExpected.GetType(), okResult.Value?.GetType());
    }

    [Fact]
    public async void GetMedicalInfo_ReturnException_WhenInputNotValid()
    {
        //Arrange
        var patientId = Guid.Empty;
        var medicalInfoId = Guid.Empty;
        _medicalInfoServiceMock.Setup(a => a.GetMedicalInfo(patientId, medicalInfoId))
            .ReturnsAsync(new Exception("L'id ne peut pas être un Guid Vide"));

        //Act
        var result = await _patientController.GetMedical(patientId, medicalInfoId);

        //Arrange
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal("L'id ne peut pas être un Guid Vide", objectResult.Value);
    }

    [Fact]
    public async void GetAll_ReturnOkResult_WithAllMedicalInfo()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoList = new List<MedicalInfoDto>
        {
            new()
            {
                Id = Guid.NewGuid(),Name = "Allergie",Note = "Allergie sur la peau",Type = MedicalInformationType.AllergiesAndReactions
            },
            new()
            {
                Id = Guid.NewGuid(),Name = "Allergie",Note = "Allergie sur la peau 2",Type = MedicalInformationType.AllergiesAndReactions
            },
        };
        _medicalInfoServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(medicalInfoList);


        //Act
        var result = await _patientController.GetMedical(patientId);
        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsAssignableFrom<List<MedicalInfoDto>>(okResult.Value);
        Assert.NotEqual(1, returnedResult.Count);
        Assert.Equal(2, returnedResult.Count);
        Assert.Equal(medicalInfoList, returnedResult);
    }

    [Fact]
    public async void GetAll_ReturnOk_WithEmptyList_WhenMedicalInfoNotFound()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoList = new List<MedicalInfoDto>();
        _medicalInfoServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(medicalInfoList);

        //Act
        var result = await _patientController.GetMedical(patientId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsAssignableFrom<List<MedicalInfoDto>>(okResult.Value);
        Assert.Empty(returnedResult);
    }

    [Fact]
    public async void CreateMedicalInfo_ReturnTrue_WhenInputIsValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoToCreate = new MedicalInfoCreateDto
        {
            Type = MedicalInformationType.AllergiesAndReactions,
            Name = "Allergie",
            Note = "Allergie de la peau",
        };
        _medicalInfoServiceMock.Setup(a => a.Create(patientId, medicalInfoToCreate)).ReturnsAsync(true);

        //Act
        var result = await _patientController.CreateMedical(patientId, medicalInfoToCreate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }


    [Fact]
    public async void CreateMedicalInfo_ReturnException_WhenModalIsNotValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoToCreate = new MedicalInfoCreateDto
        {
            Type = MedicalInformationType.AllergiesAndReactions,
            Note = "Allergie de la peau",
        };
        _patientController.ModelState.AddModelError("Name", "First Name is Required");
        _medicalInfoServiceMock.Setup(a => a.Create(patientId, medicalInfoToCreate)).ReturnsAsync(false);

        //Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(() => _patientController.CreateMedical(patientId, medicalInfoToCreate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdateMedicalInfo_ReturnTrue_WhenMedicalInfoExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var updatedMedicalInfo = new MedicalInfoPatchDto
        {
            Name = "Allergie",
            Note = "Allergie de la peau",
            Type = MedicalInformationType.AllergiesAndReactions
        };
        _medicalInfoServiceMock.Setup(a => a.Patch(patientId, medicalInfoId, updatedMedicalInfo)).ReturnsAsync(true);
        //Act
        var result = await _patientController.PatchMedical(patientId, medicalInfoId, updatedMedicalInfo);
        //Arrange
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void UpdateMedicalInfo_ReturnException_WhenMedicalInfoIsNotValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var updatedMedicalInfo = new MedicalInfoPatchDto
        {
            Name = "Allergie",
            Note = "Allergie de la peau"
        };

        _patientController.ModelState.AddModelError("Type", "Type Name is Required");
        _medicalInfoServiceMock.Setup(a => a.Patch(patientId, medicalInfoId, updatedMedicalInfo)).ReturnsAsync(false);

        //Act & Arrange
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.PatchMedical(patientId, medicalInfoId, updatedMedicalInfo));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdateMedicalInfo_ReturnException_WhenMedicalInfoDoesNotExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var updatedMedicalInfo = new MedicalInfoPatchDto
        {
            Name = "Allergie",
            Note = "Allergie de la peau",
            Type = MedicalInformationType.AllergiesAndReactions
        };

        _medicalInfoServiceMock.Setup(a => a.Patch(patientId, medicalInfoId, updatedMedicalInfo))
            .ReturnsAsync(new Exception($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.PatchMedical(patientId, medicalInfoId, updatedMedicalInfo);

        //Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!", objectResult.Value);
    }


    [Fact]
    public async void DeleteMedicalInfo_ReturnTrue_WhenMedicalInfoExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        _medicalInfoServiceMock.Setup(a => a.Delete(patientId, medicalInfoId)).ReturnsAsync(true);

        //Act
        var result = await _patientController.DeleteMedical(patientId, medicalInfoId);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, checkResult.StatusCode);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void DeleteContact_ReturnTrue_WhenContactDoesNotExist()
    {
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        _medicalInfoServiceMock.Setup(a => a.Delete(patientId, medicalInfoId))
            .ReturnsAsync(new Exception($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.DeleteMedical(patientId, medicalInfoId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!", objectResult.Value);
    }
}
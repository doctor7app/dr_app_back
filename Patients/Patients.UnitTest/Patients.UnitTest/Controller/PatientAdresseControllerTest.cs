using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Api.Controllers;
using Patients.Application.DTOs.Adresse;
using Patients.Application.Interfaces;

namespace Patients.UnitTest.Controller;

/// <summary>
/// This test class , check the relation ship between Patient and adresse
/// </summary>
public class PatientAdresseControllerTest
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IContactService> _contractServiceMock;
    private readonly Mock<IAdresseService> _adresseServiceMock;
    private readonly Mock<IMedicalInfoService> _medicalInfoServiceMock;
    private readonly PatientsController _patientController;

    public PatientAdresseControllerTest()
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
    public async void GetAdresse_ReturnAdresse_WhenAdressExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var expectedAdress = new AdresseDto
        {
            Id = Guid.NewGuid(),
            AdditionalInformation = "Third Floor",
            City = "Nabeul",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul",
            Street = "20 rue mahmoud",
            Type = AdresseType.Home
        };
        _adresseServiceMock.Setup(a => a.GetAdresse(patientId, adresseId)).ReturnsAsync(expectedAdress);

        //Act
        var result = await _patientController.GetAdresse(patientId, adresseId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(expectedAdress, okResult.Value);
        Assert.Equal(expectedAdress.GetType(), okResult.Value?.GetType());
    }

    [Fact]
    public async void GetAdresse_ReturnException_WhenInputInvalid()
    {
        //Arrange
        var patientId = Guid.Empty;
        var adresseId = Guid.Empty;
        
        _adresseServiceMock.Setup(a => a.GetAdresse(patientId, adresseId))
            .ReturnsAsync(new Exception("L'id ne peut pas être un Guid Vide"));

        //Act
        var result = await _patientController.GetAdresse(patientId, adresseId);

        //Assert

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal("L'id ne peut pas être un Guid Vide", objectResult.Value);
    }

    [Fact]
    public async void GetAll_ReturnOkResult_WithListOfAdresse()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var adresseList = new List<AdresseDto>
        {
            new (){Id = Guid.NewGuid(),Type = AdresseType.Home,Street = "20 rue de france"},
            new (){Id = Guid.NewGuid(),Type = AdresseType.Work,Street = "20 rue de france"}
        };

        _adresseServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(adresseList);

        //Act 
        var result = await _patientController.GetAdresse(patientId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPatients = Assert.IsAssignableFrom<List<AdresseDto>>(okResult.Value);
        Assert.Equal(2, returnedPatients.Count);
        Assert.Equal(adresseList, returnedPatients);
    }


    [Fact]
    public async void GetAll_ReturnOkResult_WithEmptyList_WhenNoAdressesFound()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var adresseList = new List<AdresseDto>();

        _adresseServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(adresseList);

        //Act 
        var result = await _patientController.GetAdresse(patientId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedPatients = Assert.IsAssignableFrom<List<AdresseDto>>(okResult.Value);
        Assert.Empty(returnedPatients);
    }

    [Fact]
    public async void CreateAdress_ReturnTrue_WhenInputIsValid()
    {
        var patientId = Guid.NewGuid();
        var adresseToCreate = new AdresseCreateDto
        {
            Street = "20 rue de france",
            Type = AdresseType.Home,
            AdditionalInformation = "Third Floor",
            City = "Nabeul",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul"
        };
        _adresseServiceMock.Setup(a => a.Create(patientId, adresseToCreate)).ReturnsAsync(true);

        //Act
        var result = await _patientController.CreateAdresse(patientId, adresseToCreate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void CreateAdresse_ReturnException_WhenModalIsNotValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var adresseToCreate = new AdresseCreateDto
        {    
            Type = AdresseType.Home,
            AdditionalInformation = "Third Floor",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul"
        };

        _patientController.ModelState.AddModelError("Street", "Street is Required");
        _patientController.ModelState.AddModelError("City", "City is Required");
        _adresseServiceMock.Setup(a => a.Create(patientId, adresseToCreate)).ReturnsAsync(true);

        //Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.CreateAdresse(patientId,adresseToCreate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
        
    }

    [Fact]
    public async void UpdateAdresse_ReturnTrue_WhenAdresseExist()
    {
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var adresseToUpdate = new AdressePatchDto
        {
            Street = "20 rue de france",
            Type = AdresseType.Home,
            AdditionalInformation = "Third Floor",
            City = "Nabeul",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul"
        };
        _adresseServiceMock.Setup(a => a.Patch(patientId, adresseId, adresseToUpdate)).ReturnsAsync(true);

        //Act
        var result = await _patientController.PatchAdresse(patientId, adresseId, adresseToUpdate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);

    }

    [Fact]
    public async void UpdateAdresse_ReturnTrue_WhenModalIsNotValid()
    {
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var adresseToUpdate = new AdressePatchDto
        {
            Type = AdresseType.Home,
            AdditionalInformation = "Third Floor",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul"
        };

        _patientController.ModelState.AddModelError("Street", "Street is Required");
        _patientController.ModelState.AddModelError("City", "City is Required");
        _adresseServiceMock.Setup(a => a.Patch(patientId, adresseId, adresseToUpdate)).ReturnsAsync(true);

        //Act & Arrange

        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.PatchAdresse(patientId,adresseId, adresseToUpdate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdateAdresse_ReturnException_WhenAdresseDoesNotExist()
    {
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var adresseToUpdate = new AdressePatchDto
        {
            Street = "20 rue de france",
            Type = AdresseType.Home,
            AdditionalInformation = "Third Floor",
            City = "Nabeul",
            Country = "Tunisia",
            PostalCode = "8000",
            Provence = "Nabeul"
        };

        _adresseServiceMock.Setup(a => a.Patch(patientId, adresseId, adresseToUpdate))
            .ReturnsAsync(new Exception($"L'élement avec l'id {adresseId} n'existe pas dans la base de données!"));

        //Act 
        var result = await _patientController.PatchAdresse(patientId, adresseId, adresseToUpdate);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {adresseId} n'existe pas dans la base de données!", objectResult.Value);
    }


    [Fact]
    public async void DeleteAdresse_ReturnTrue_WhenAdresseExist()
    {
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        _adresseServiceMock.Setup(a => a.Delete(patientId, adresseId)).ReturnsAsync(true);

        //Act
        var result = await _patientController.DeleteAdresse(patientId, adresseId);

        //Assert

        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, checkResult.StatusCode);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void DeleteAdresse_ReturnTrue_WhenAdresseDoesNotExist()
    {
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        _adresseServiceMock.Setup(a => a.Delete(patientId, adresseId))
            .ReturnsAsync(new Exception($"L'élement avec l'id {adresseId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.DeleteAdresse(patientId, adresseId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {adresseId} n'existe pas dans la base de données!", objectResult.Value);
    }
}
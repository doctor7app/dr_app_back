using Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patients.Api.Controllers;
using Patients.Application.DTOs.Contact;
using Patients.Application.Interfaces;

namespace Patients.UnitTest.Controllers;

/// <summary>
/// This class test the parent child relation ship between Patient and contact
/// </summary>
public class PatientContactControllerTest
{
    private readonly Mock<IPatientService> _patientServiceMock;
    private readonly Mock<IContactService> _contactServiceMock;
    private readonly Mock<IAdresseService> _adresseServiceMock;
    private readonly Mock<IMedicalInfoService> _medicalInfoServiceMock;
    private readonly PatientsController _patientController;

    public PatientContactControllerTest()
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
    public async void GetContact_ReturnContact_WhenContactExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var contactExpected = new ContactDto
        {
            Id = contactId,
            Email = "Joe.Lancelot@gmail.com",
            FirstName = "Joe",
            LastName = "Lancelot",
            PhoneNumber = "98900900",
            Type = ContactType.Personnel
        };
        _contactServiceMock.Setup(a => a.GetContact(patientId, contactId)).ReturnsAsync(contactExpected);

        //Act
        var result = await _patientController.GetContact(patientId, contactId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult);
        Assert.Equal(contactExpected, okResult.Value);
        Assert.Equal(contactExpected.GetType(), okResult.Value?.GetType());

    }

    [Fact]
    public async void GetContact_ReturnException_WhenInputNotValid()
    {
        //Arrange
        var patientId = Guid.Empty;
        var adresseId = Guid.Empty;

        _contactServiceMock.Setup(a => a.GetContact(patientId, adresseId))
            .ReturnsAsync(new Exception("L'id ne peut pas être un Guid Vide"));

        //Act
        var result = await _patientController.GetContact(patientId, adresseId);

        //Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal("L'id ne peut pas être un Guid Vide", objectResult.Value);

    }

    [Fact]
    public async void GetAll_ReturnOkResult_WithAllContact()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactList = new List<ContactDto>
        {
            new()
            {
                FirstName = "Joe", LastName = "LanceLot", Email = "Joe@gmail.com", Id = Guid.NewGuid(),
                Type = ContactType.Personnel, PhoneNumber = "90900900"
            },
            new()
            {
                FirstName = "Joe", LastName = "Bernard", Email = "Bernard@gmail.com", Id = Guid.NewGuid(),
                Type = ContactType.Work, PhoneNumber = "90800600"
            },
            new()
            {
                FirstName = "Walker", LastName = "Smith", Email = "Walker@gmail.com", Id = Guid.NewGuid(),
                Type = ContactType.Personnel, PhoneNumber = "96600600"
            }
        };
        _contactServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(contactList);


        //Act
        var result = await _patientController.GetContact(patientId);
        //Assert

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsAssignableFrom<List<ContactDto>>(okResult.Value);
        Assert.NotEqual(2, returnedResult.Count);
        Assert.Equal(3, returnedResult.Count);
        Assert.Equal(contactList, returnedResult);
    }

    [Fact]
    public async void GetAll_ReturnOk_WithEmptyList_WhenContactNotFound()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactList = new List<ContactDto>();
        _contactServiceMock.Setup(a => a.GetRelative(patientId)).ReturnsAsync(contactList);

        //Act
        var result = await _patientController.GetContact(patientId);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsAssignableFrom<List<ContactDto>>(okResult.Value);
        Assert.Empty(returnedResult);

    }

    [Fact]
    public async void CreateContact_ReturnTrue_WhenInputIsValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactToCreate = new ContactCreateDto
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Type = ContactType.Personnel,
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "25100100"
        };
        _contactServiceMock.Setup(a => a.Create(patientId, contactToCreate)).ReturnsAsync(true);

        //Act
        var result = await _patientController.CreateContact(patientId, contactToCreate);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void CreateContact_ReturnException_WhenModalIsNotValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactToCreate = new ContactCreateDto
        {
            Type = ContactType.Personnel,
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "25100100"
        };
        _patientController.ModelState.AddModelError("FirstName", "First Name is Required");
        _patientController.ModelState.AddModelError("LastName", "Last Name is Required");
        _contactServiceMock.Setup(a => a.Create(patientId, contactToCreate)).ReturnsAsync(false);

        //Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(() => _patientController.CreateContact(patientId, contactToCreate));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdateContact_ReturnTrue_WhenContactExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var updatedContact = new ContactPatchDto
        {
            LastName = "Joe",
            FirstName = "Lancelot",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "97300300",
            Type = ContactType.Work
        };
        _contactServiceMock.Setup(a => a.Patch(patientId, contactId, updatedContact)).ReturnsAsync(true);
        //Act
        var result = await _patientController.PatchContact(patientId, contactId, updatedContact);
        //Arrange
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(checkResult);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void UpdateContact_ReturnException_WhenModalIsNotValid()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var updatedContact = new ContactPatchDto
        {
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "97300300",
            Type = ContactType.Work
        };

        _patientController.ModelState.AddModelError("LastName", "Last Name is Required");
        _patientController.ModelState.AddModelError("FirstName", "First Name is Required");
        _contactServiceMock.Setup(a => a.Patch(patientId, contactId, updatedContact)).ReturnsAsync(false);

        //Act & Arrange
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientController.PatchContact(patientId, contactId, updatedContact));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async void UpdateContact_ReturnException_WhenContactDoesNotExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var updatedContact = new ContactPatchDto
        {
            LastName = "Joe",
            FirstName ="Lancelot",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "97300300",
            Type = ContactType.Work
        };

        _contactServiceMock.Setup(a=>a.Patch(patientId,contactId,updatedContact))
            .ReturnsAsync(new Exception($"L'élement avec l'id {contactId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.PatchContact(patientId, contactId, updatedContact);

        //Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {contactId} n'existe pas dans la base de données!", objectResult.Value);
    }

    [Fact]
    public async void DeleteContact_ReturnTrue_WhenContactExist()
    {
        //Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _contactServiceMock.Setup(a => a.Delete(patientId, contactId)).ReturnsAsync(true);

        //Act
        var result = await _patientController.DeleteContact(patientId, contactId);

        //Assert
        var checkResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, checkResult.StatusCode);
        Assert.Equal(true, checkResult.Value);
    }

    [Fact]
    public async void DeleteContact_ReturnTrue_WhenContactDoesNotExist()
    {
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _contactServiceMock.Setup(a => a.Delete(patientId, contactId))
            .ReturnsAsync(new Exception($"L'élement avec l'id {contactId} n'existe pas dans la base de données!"));

        //Act
        var result = await _patientController.DeleteContact(patientId, contactId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal($"L'élement avec l'id {contactId} n'existe pas dans la base de données!", objectResult.Value);
    }
}
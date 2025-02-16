using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Implementation;
using Patients.Infrastructure.Persistence;
using System.Linq.Expressions;
using Contracts.Messages.Patients;
using Patients.Application.DTOs.Contact;
using MassTransit;
using Common.Enums.Patients;

namespace Patients.UnitTest.Services;

public class ContactServiceTests
{
    private readonly IContactService _contactService;
    private readonly Mock<IRepository<Contact, PatientDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;

    public ContactServiceTests()
    {
        _mockRepository = new Mock<IRepository<Contact, PatientDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _contactService = new ContactService(_mockRepository.Object, _mockMapper.Object, _mockPublishEndpoint.Object);
    }

    #region Get

    [Fact]
    public async Task GetContact_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var contactId = Guid.Empty;
        var patientId = Guid.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.GetContact(patientId, contactId));
        Assert.Equal("L'id ne peut pas être un Guid Vide", exception.Message);
    }

    [Fact]
    public async Task GetContact_ShouldReturnDto_WhenEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var entity = new Contact
        {
            ContactId = contactId,
            FkIdPatient = patientId,
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            Type = ContactType.Personnel,

        };
        var expectedDto = new ContactDto
        {
            Id = id,
            LastName = "Joe",
            FirstName = "Lancelot",
            Email = "Joe.Lancelot@gmail.com",
            Type = ContactType.Personnel,
            PhoneNumber = "98600600"
        };


        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Contact, bool>>>(expr => expr.Compile()(entity)),
            It.IsAny<Func<IQueryable<Contact>, IIncludableQueryable<Contact, object>>>()
        )).ReturnsAsync(entity);


        _mockMapper.Setup(m => m.Map<ContactDto>(entity)).Returns(expectedDto);

        // Act
        var result = await _contactService.GetContact(patientId, contactId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ContactDto>(result);
        Assert.Equal(expectedDto, result);
    }
    
    [Fact]
    public async Task GetContact_ShouldReturnListOfDtos_WhenCalled()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var entities = new List<Contact>
        {
            new() { ContactId = Guid.NewGuid(), LastName = "Doe", FirstName = "John" },
            new() { ContactId = Guid.NewGuid(), LastName = "Smith", FirstName = "Jane" }
        };
        var expectedDtos = new List<ContactDto>
        {
            new() { Id = entities[0].ContactId, LastName = "Doe", FirstName = "John" },
            new() { Id = entities[1].ContactId, LastName = "Smith", FirstName = "Jane" }
        };

        _mockRepository.Setup(u => u.GetListAsync(
            It.IsAny<Expression<Func<Contact, bool>>>(),
            It.IsAny<Func<IQueryable<Contact>, IIncludableQueryable<Contact, object>>>()
        )).ReturnsAsync(entities);

        _mockMapper.Setup(m => m.Map<IEnumerable<ContactDto>>(entities)).Returns(expectedDtos);

        // Act
        var result = await _contactService.GetRelative(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ContactDto>>(result);
        var resultDto = result.ToList();
        Assert.Equal(expectedDtos, resultDto);
        Assert.IsAssignableFrom<IEnumerable<ContactDto>>(result);
        Assert.Equal(expectedDtos.Count, resultDto.Count());
        Assert.Equal(expectedDtos[0].Id, resultDto.ElementAt(0).Id);
        Assert.Equal(expectedDtos[1].Id, resultDto.ElementAt(1).Id);
    }

    #endregion

    #region Create

    [Fact]
    public async Task CreateContact_ShouldAddEntityAndPublishEvent_WhenCalled()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var entityDto = new ContactCreateDto
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            Type = ContactType.Personnel
        };
        var entity = new Contact
        {
            ContactId = contactId,
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            Type = ContactType.Personnel,
            FkIdPatient = patientId
        };
        var entityCreatedEvent = new ContactCreatedEvent()
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            Type = ContactType.Personnel,
            ContactId = contactId
        };

        _mockMapper.Setup(m => m.Map<Contact>(entityDto)).Returns(entity);
        _mockMapper.Setup(m => m.Map<ContactCreatedEvent>(It.IsAny<PatientDto>())).Returns(entityCreatedEvent);
        _mockRepository.Setup(u => u.AddAsync(entity)).Returns(Task.CompletedTask);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        // Act
        var result = await _contactService.Create(patientId, entityDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(true, result);
        _mockRepository.Verify(u => u.AddAsync(entity), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(entityCreatedEvent, It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task CreateContact_ShouldThrowException_WhenSaveFails()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var entityDto = new ContactCreateDto() { };
        var entity = new Contact() { };
        _mockMapper.Setup(m => m.Map<Contact>(entityDto)).Returns(entity);
        _mockRepository.Setup(u => u.AddAsync(entity)).Returns(Task.CompletedTask);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Create(patientId,entityDto));
        Assert.Equal("Could not save Contact to database", exception.Message);
        _mockRepository.Verify(u => u.AddAsync(entity), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<ContactCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Update

    [Fact]
    public async Task PatchContact_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var contactId = Guid.Empty;
        var patientId = Guid.Empty;
        var patchDto = new ContactPatchDto() { };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Patch(patientId,contactId, patchDto));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async Task PatchContact_ShouldThrowException_WhenEntityToUpdateNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var patchDto = new ContactPatchDto { };
        _mockRepository.Setup(u => u.GetAsync(a => a.ContactId == contactId && a.FkIdPatient == patientId, null))
            .ReturnsAsync((Contact)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Patch(patientId,contactId, patchDto));
        Assert.Equal($"L'élement avec l'id {contactId} n'existe pas dans la base de données!", exception.Message);
    }

    [Fact]
    public async Task PatchContact_ShouldThrowException_WhenUpdateFails()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        var patchDto = new ContactPatchDto { };
        var entityToUpdate = new Contact { ContactId = contactId };

        // Setup the unit of work mock to return the entity
        _mockRepository.Setup(u => u.GetAsync(a => a.ContactId == contactId&& a.FkIdPatient == patientId, null))
            .ReturnsAsync(entityToUpdate);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Patch(patientId, contactId, patchDto));
        Assert.Equal("Could not update Contact to database", exception.Message);
    }

    [Fact]
    public async Task PatchContact_ShouldUpdateEntityAndPublishEvent_WhenSuccessful()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var patchDto = new ContactPatchDto { };
        var entityToUpdate = new Contact { ContactId = contactId,FkIdPatient = patientId};
        var updatedDto = new ContactDto { };
        var updatedEvent = new ContactUpdatedEvent { };

        _mockRepository.Setup(u => u.GetAsync(a => a.ContactId == contactId && a.FkIdPatient == patientId, null))
            .ReturnsAsync(entityToUpdate);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<ContactDto>(entityToUpdate)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<ContactUpdatedEvent>(updatedDto)).Returns(updatedEvent);

        // Act
        var result = await _contactService.Patch(patientId,contactId, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(a => a.ContactId == contactId && a.FkIdPatient == patientId, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(updatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Delete
    [Fact]
    public async Task DeleteContact_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var contactId = Guid.Empty;
        var patientId = Guid.Empty;
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Delete(patientId,contactId));
        Assert.Equal("L'id ne peut pas être un Guid Vide", exception.Message);
    }

    [Fact]
    public async Task DeleteContact_ShouldThrowException_WhenPatientDoesNotExist()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var entity = new Contact { ContactId = contactId,FkIdPatient = patientId};
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Contact, bool>>>(expr => expr.Compile()(entity)),
            It.IsAny<Func<IQueryable<Contact>, IIncludableQueryable<Contact, object>>>()
        ))!.ReturnsAsync((Contact)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Delete(patientId,contactId));
        Assert.Equal($"L'élement avec l'id {contactId} n'existe pas dans la base de données!", exception.Message);
    }

    [Fact]
    public async Task DeleteContact_ShouldThrowException_WhenDeletionFails()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var patient = new Contact { ContactId = contactId,FkIdPatient = patientId};
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Contact, bool>>>(expr => expr.Compile()(patient)),
            It.IsAny<Func<IQueryable<Contact>, IIncludableQueryable<Contact, object>>>()
        ))!.ReturnsAsync(patient);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _contactService.Delete(patientId,contactId));
        Assert.Equal("Could not Delete Contact from database", exception.Message);
    }

    [Fact]
    public async Task DeleteContact_ShouldReturnTrue_WhenPatientIsDeletedSuccessfully()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var patient = new Contact() { ContactId = contactId,FkIdPatient = patientId};
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Contact, bool>>>(expr => expr.Compile()(patient)),
            It.IsAny<Func<IQueryable<Contact>, IIncludableQueryable<Contact, object>>>()
        ))!.ReturnsAsync(patient);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        // Act
        var result = await _contactService.Delete(patientId,contactId);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(patient), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<ContactDeletedEvent>(), default), Times.Once);
    }
    #endregion
}
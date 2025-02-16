using System.Linq.Expressions;
using AutoMapper;
using Common.Enums;
using Common.Services.Interfaces;
using Contracts.Messages.Patients;
using MassTransit;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Implementation;
using Patients.Infrastructure.Persistence;

namespace Patients.UnitTest.Services;

public class PatientServiceTests
{
    private readonly IPatientService _patientService;
    private readonly Mock<IRepository<Patient, PatientDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;

    public PatientServiceTests()
    {
        _mockRepository = new Mock<IRepository<Patient, PatientDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _patientService = new PatientService(_mockRepository.Object, _mockMapper.Object, _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task GetPatient_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var emptyId = Guid.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Get(emptyId));
        Assert.Equal("L'id ne peut pas être un Guid Vide", exception.Message);
    }

    [Fact]
    public async Task GetPatient_ShouldReturnDto_WhenEntityExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Patient
        {
            PatientId = id,
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            Gender = Gender.Male,
            HomeNumber = "72200200",
            MiddleName = "",
            SocialSecurityNumber = "1234856"
        };
        var expectedDto = new PatientDto
        {
            Id = id,
            LastName = "Joe",
            FirstName = "Lancelot"
        };


        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Patient, bool>>>(expr => expr.Compile()(entity)), 
            It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>()
        )).ReturnsAsync(entity);

        
        _mockMapper.Setup(m => m.Map<PatientDto>(entity)).Returns(expectedDto);

        // Act
        var result = await _patientService.Get(id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PatientDto>(result);
        Assert.Equal(expectedDto, result);
    }

    [Fact]
    public async Task GetPatient_ShouldReturnListOfDtos_WhenCalled()
    {
        // Arrange
        var entities = new List<Patient>
        {
            new() { PatientId = Guid.NewGuid(), LastName = "Doe", FirstName = "John" },
            new() { PatientId = Guid.NewGuid(), LastName = "Smith", FirstName = "Jane" }
        };
        var expectedDtos = new List<PatientDto>
        {
            new() { Id = entities[0].PatientId, LastName = "Doe", FirstName = "John" },
            new() { Id = entities[1].PatientId, LastName = "Smith", FirstName = "Jane" }
        };

        _mockRepository.Setup(u => u.GetListAsync(
            It.IsAny<Expression<Func<Patient, bool>>>(),
            It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>() 
        )).ReturnsAsync(entities);

        _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(entities)).Returns(expectedDtos);

        // Act
        var result = await _patientService.Get();

        // Assert
        Assert.NotNull(result); 
        Assert.IsAssignableFrom<IEnumerable<PatientDto>>(result);
        var patientDtos = result.ToList();
        Assert.Equal(expectedDtos, patientDtos);
        Assert.IsAssignableFrom<IEnumerable<PatientDto>>(result);
        Assert.Equal(expectedDtos.Count, patientDtos.Count());
        Assert.Equal(expectedDtos[0].Id, patientDtos.ElementAt(0).Id);
        Assert.Equal(expectedDtos[1].Id, patientDtos.ElementAt(1).Id);
    }

    [Fact]
    public async Task CreatePatient_ShouldAddEntityAndPublishEvent_WhenCalled()
    {
        // Arrange
        var entityDto = new PatientCreateDto
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            BirthDate = new DateTime(1952,01,01),
            DeathDate = null,
            Gender = Gender.Male,
            HomeNumber = "72200200",
            MiddleName = "",
            SocialNumber = "1234856"
        };
        var entity = new Patient
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            Gender = Gender.Male,
            HomeNumber = "72200200",
            MiddleName = "",
            SocialSecurityNumber = "1234856",
        };
        var entityCreatedEvent = new PatientCreatedEvent
        {
            LastName = "Lancelot",
            FirstName = "Joe",
            Email = "Joe.Lancelot@gmail.com",
            PhoneNumber = "98600600",
            BirthDate = new DateTime(1952, 01, 01),
            DeathDate = null,
            Gender = Gender.Male,
            HomeNumber = "72200200",
            MiddleName = "",
            SocialNumber = "1234856"
        };

        _mockMapper.Setup(m => m.Map<Patient>(entityDto)).Returns(entity);
        _mockMapper.Setup(m => m.Map<PatientCreatedEvent>(It.IsAny<PatientDto>())).Returns(entityCreatedEvent);
        _mockRepository.Setup(u => u.AddAsync(entity)).Returns(Task.CompletedTask);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        // Act
        var result = await _patientService.Create(entityDto);

        // Assert
        Assert.NotNull(result); 
        Assert.Equal(true,result);
        _mockRepository.Verify(u => u.AddAsync(entity), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(entityCreatedEvent, It.IsAny<CancellationToken>()), Times.Once);
        
    }

    [Fact]
    public async Task CreatePatientAsync_ShouldThrowException_WhenSaveFails()
    {
        // Arrange
        var entityDto = new PatientCreateDto();
        var entity = new Patient();
        _mockMapper.Setup(m => m.Map<Patient>(entityDto)).Returns(entity);
        _mockRepository.Setup(u => u.AddAsync(entity)).Returns(Task.CompletedTask);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0); 

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Create(entityDto));
        Assert.Equal("Could not save Patient to database", exception.Message);
        _mockRepository.Verify(u => u.AddAsync(entity), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<PatientCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task PatchPatient_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var emptyKey = Guid.Empty;
        var patchDto = new PatientPatchDto();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Patch(emptyKey, patchDto));
        Assert.Equal("Merci de vérifier les données saisie !", exception.Message);
    }

    [Fact]
    public async Task PatchPatient_ShouldThrowException_WhenEntityToUpdateNotFound()
    {
        // Arrange
        var key = Guid.NewGuid();
        var patchDto = new PatientPatchDto();
        _mockRepository.Setup(u => u.GetAsync(a => a.PatientId == key,null)).ReturnsAsync((Patient)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Patch(key, patchDto));
        Assert.Equal($"L'élement avec l'id {key} n'existe pas dans la base de données!", exception.Message);
    }

    [Fact]
    public async Task PatchPatient_ShouldThrowException_WhenUpdateFails()
    {
        // Arrange
        var key = Guid.NewGuid();
        var patchDto = new PatientPatchDto() ;
        var entityToUpdate = new Patient { PatientId = key };

        // Setup the unit of work mock to return the entity
        _mockRepository.Setup(u => u.GetAsync(a=>a.PatientId == key,null)).ReturnsAsync(entityToUpdate);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Patch(key, patchDto));
        Assert.Equal("Could not update Patient to database", exception.Message);
    }

    [Fact]
    public async Task PatchPatient_ShouldUpdateEntityAndPublishEvent_WhenSuccessful()
    {
        // Arrange
        var key = Guid.NewGuid();
        var patchDto = new PatientPatchDto ();
        var entityToUpdate = new Patient { PatientId = key }; 
        var updatedDto = new PatientDto ();
        var updatedEvent = new PatientUpdatedEvent ();

        _mockRepository.Setup(u => u.GetAsync(a=>a.PatientId == key,null)).ReturnsAsync(entityToUpdate);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<PatientDto>(entityToUpdate)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<PatientUpdatedEvent>(updatedDto)).Returns(updatedEvent);

        // Act
        var result = await _patientService.Patch(key, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(a => a.PatientId == key, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(updatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var id = Guid.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Delete(id));
        Assert.Equal("L'id ne peut pas être un Guid Vide", exception.Message);
    }

    [Fact]
    public async Task Delete_ShouldThrowException_WhenPatientDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new Patient() { PatientId = id };
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Patient, bool>>>(expr => expr.Compile()(entity)),
            It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>()
        )).ReturnsAsync((Patient)null!);
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Delete(id));
        Assert.Equal($"L'élement avec l'id {id} n'existe pas dans la base de données!", exception.Message);
    }

    [Fact]
    public async Task Delete_ShouldThrowException_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var patient = new Patient { PatientId = id };
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Patient, bool>>>(expr => expr.Compile()(patient)),
            It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>()
        )).ReturnsAsync(patient);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _patientService.Delete(id));
        Assert.Equal("Could not Delete Patient from database", exception.Message);
    }

    [Fact]
    public async Task Delete_ShouldReturnTrue_WhenPatientIsDeletedSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var patient = new Patient { PatientId = id };
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Patient, bool>>>(expr => expr.Compile()(patient)),
            It.IsAny<Func<IQueryable<Patient>, IIncludableQueryable<Patient, object>>>()
        )).ReturnsAsync(patient);
        _mockRepository.Setup(u => u.Complete()).ReturnsAsync(1);

        // Act
        var result = await _patientService.Delete(id);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(patient), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<PatientDeletedEvent>(), default), Times.Once);
    }

}
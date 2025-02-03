using System.Linq.Expressions;
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Contracts.Messages.Consultations;
using Dme.Application.DTOs.Consultations;
using Dme.Domain.Models;
using Dme.Infrastructure.Implementation;
using Dme.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Dme.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the ConsultationService class.
/// </summary>
public class ConsultationServiceTests
{
    private readonly Mock<IRepository<Consultations, DmeDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly ConsultationService _service;

    /// <summary>
    /// Initializes a new instance of the ConsultationServiceTests class.
    /// </summary>
    public ConsultationServiceTests()
    {
        _mockRepository = new Mock<IRepository<Consultations, DmeDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _service = new ConsultationService(_mockRepository.Object, _mockMapper.Object, _mockPublishEndpoint.Object);
    }

    #region Consutlation Dme Implementation

    /// <summary>
    /// Tests the GetConsultationForDme method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetConsultationForDme_ValidId_ReturnsConsultationsReadDto()
    {
        // Arrange
        var idDme = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        var consultation = new Consultations { ConsultationId = idConsultation, FkIdDme = idDme };
        var expectedDto = new ConsultationsReadDto();
        _mockRepository.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<Consultations, bool>>>(expr => expr.Compile()(consultation)),
            It.IsAny<Func<IQueryable<Consultations>, IIncludableQueryable<Consultations, object>>>()))
            .ReturnsAsync(consultation);
        _mockMapper.Setup(m => m.Map<ConsultationsReadDto>(consultation)).Returns(expectedDto);

        // Act
        var result = await _service.GetConsultationForDme(idDme, idConsultation);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetConsultationForDme method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetConsultationForDme_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetConsultationForDme(invalidId, invalidId));
    }

    /// <summary>
    /// Tests the GetAllConsultationForDme method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetAllConsultationForDme_ValidId_ReturnsListOfConsultationsReadDto()
    {
        // Arrange
        var idDme = Guid.NewGuid();
        var consultations = new List<Consultations> { new Consultations() };
        var expectedDtos = new List<ConsultationsReadDto> { new ConsultationsReadDto() };
        _mockRepository.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<Consultations, bool>>>(),
            It.IsAny<Func<IQueryable<Consultations>, IIncludableQueryable<Consultations, object>>>()))
            .ReturnsAsync(consultations);
        _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationsReadDto>>(consultations)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllConsultationForDme(idDme);

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the CreateConsultationForDme method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateConsultationForDme_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idDme = Guid.NewGuid();
        var createDto = new ConsultationsCreateDto { IdDme = idDme };
        var consultation = new Consultations();
        var entityCreatedEvent = new ConsultationCreatedEvent();

        _mockMapper.Setup(m => m.Map<Consultations>(createDto)).Returns(consultation);
        _mockMapper.Setup(m => m.Map<ConsultationCreatedEvent>(It.IsAny<ConsultationsReadDto>())).Returns(entityCreatedEvent);
        _mockRepository.Setup(repo => repo.AddAsync(consultation)).Returns(Task.CompletedTask);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateConsultationForDme(idDme, createDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.AddAsync(consultation), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(entityCreatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests the CreateConsultationForDme method with invalid data.
    /// </summary>
    [Fact]
    public async Task CreateConsultationForDme_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new ConsultationsCreateDto(); // Invalid, IdDme is null

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateConsultationForDme(Guid.NewGuid(), createDto));
    }

    /// <summary>
    /// Tests the PatchConsultationForDme method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchConsultationForDme_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idDme = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        var patchDto = new ConsultationsPatchDto();
        var entityDto = new ConsultationsReadDto();
        var consultation = new Consultations { ConsultationId = idConsultation, FkIdDme = idDme };
        var updatedEvent = new ConsultationUpdatedEvent();

        _mockRepository.Setup(u => u.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme,null))
            .ReturnsAsync(consultation);
        _mockMapper.Setup(m => m.Map<ConsultationsPatchDto>(consultation)).Returns(patchDto);
        _mockMapper.Setup(m => m.Map<ConsultationsReadDto>(consultation)).Returns(entityDto);
        _mockMapper.Setup(m => m.Map<ConsultationUpdatedEvent>(entityDto)).Returns(updatedEvent);
        patchDto.UpdateWithDto(consultation);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.PatchConsultationForDme(idDme, idConsultation, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(updatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests the PatchConsultationForDme method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task PatchConsultationForDme_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var patchDto = new ConsultationsPatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.PatchConsultationForDme(invalidId, invalidId, patchDto));
    }

    /// <summary>
    /// Tests the DeleteConsultationForDme method with a valid ID.
    /// </summary>
    [Fact]
    public async Task DeleteConsultationForDme_ValidId_ReturnsSuccess()
    {
        // Arrange
        var idDme = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        var consultation = new Consultations { ConsultationId = idConsultation, FkIdDme = idDme };
        _mockRepository.Setup(u => u.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme, null))
            .ReturnsAsync(consultation);
        _mockRepository.Setup(repo => repo.Remove(consultation));
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteConsultationForDme(idDme, idConsultation);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(consultation), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<ConsultationDeletedEvent>(), default), Times.Once);
    }

    /// <summary>
    /// Tests the DeleteConsultationForDme method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task DeleteConsultationForDme_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteConsultationForDme(invalidId, invalidId));
    }

    #endregion

    #region Consultation

    /// <summary>
    /// Tests the GetConsultationById method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetConsultationById_ValidId_ReturnsConsultationsReadDto()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var consultation = new Consultations { ConsultationId = idConsultation };
        var expectedDto = new ConsultationsReadDto();
        _mockRepository.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<Consultations, bool>>>(expr => expr.Compile()(consultation)),
            It.IsAny<Func<IQueryable<Consultations>, IIncludableQueryable<Consultations, object>>>()))
            .ReturnsAsync(consultation);
        _mockMapper.Setup(m => m.Map<ConsultationsReadDto>(consultation)).Returns(expectedDto);

        // Act
        var result = await _service.GetConsultationById(idConsultation);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetConsultationById method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetConsultationById_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetConsultationById(invalidId));
    }

    /// <summary>
    /// Tests the GetAllConsultation method.
    /// </summary>
    [Fact]
    public async Task GetAllConsultation_ReturnsListOfConsultationsReadDto()
    {
        // Arrange
        var consultations = new List<Consultations> { new Consultations() };
        var expectedDtos = new List<ConsultationsReadDto> { new ConsultationsReadDto() };
        
        _mockRepository.Setup(u => u.GetListAsync(
            It.IsAny<Expression<Func<Consultations, bool>>>(),
            It.IsAny<Func<IQueryable<Consultations>, IIncludableQueryable<Consultations, object>>>()
        )).ReturnsAsync(consultations);

        _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationsReadDto>>(consultations)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllConsultation();

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the CreateConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateConsultation_ValidData_ReturnsSuccess()
    {
        // Arrange
        var createDto = new ConsultationsCreateDto { IdDme = Guid.NewGuid() };
        var consultation = new Consultations();
        var entityCreatedEvent = new ConsultationCreatedEvent();

        _mockMapper.Setup(m => m.Map<Consultations>(createDto)).Returns(consultation);
        _mockMapper.Setup(m => m.Map<ConsultationCreatedEvent>(It.IsAny<ConsultationsReadDto>())).Returns(entityCreatedEvent);
        _mockRepository.Setup(repo => repo.AddAsync(consultation)).Returns(Task.CompletedTask);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateConsultation(createDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.AddAsync(consultation), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(entityCreatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests the CreateConsultation method with invalid data.
    /// </summary>
    [Fact]
    public async Task CreateConsultation_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new ConsultationsCreateDto(); // Invalid, IdDme is null

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateConsultation(createDto));
    }

    /// <summary>
    /// Tests the PatchConsultationById method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchConsultationById_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var patchDto = new ConsultationsPatchDto();
        var consultation = new Consultations { ConsultationId = idConsultation };
        var updatedEvent = new ConsultationUpdatedEvent();
        var entityDto = new ConsultationsReadDto();

        _mockRepository.Setup(u => u.GetAsync(x => x.ConsultationId == idConsultation, null))
            .ReturnsAsync(consultation);
        
        _mockMapper.Setup(m => m.Map<ConsultationsReadDto>(consultation)).Returns(entityDto);
        _mockMapper.Setup(m => m.Map<ConsultationUpdatedEvent>(entityDto)).Returns(updatedEvent);

        _mockMapper.Setup(m => m.Map<ConsultationsPatchDto>(consultation)).Returns(patchDto);
        patchDto.UpdateWithDto(consultation);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.PatchConsultationById(idConsultation, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(x => x.ConsultationId == idConsultation, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(updatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests the PatchConsultationById method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task PatchConsultationById_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var patchDto = new ConsultationsPatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.PatchConsultationById(invalidId, patchDto));
    }

    /// <summary>
    /// Tests the DeleteConsultationById method with a valid ID.
    /// </summary>
    [Fact]
    public async Task DeleteConsultationById_ValidId_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var consultation = new Consultations { ConsultationId = idConsultation };
        _mockRepository.Setup(u => u.GetAsync(x => x.ConsultationId == idConsultation,null))
            .ReturnsAsync(consultation);
        _mockRepository.Setup(repo => repo.Remove(consultation));
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteConsultationById(idConsultation);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(consultation), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<ConsultationDeletedEvent>(), default), Times.Once);
    }

    /// <summary>
    /// Tests the DeleteConsultationById method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task DeleteConsultationById_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteConsultationById(invalidId));
    }

    #endregion 
}
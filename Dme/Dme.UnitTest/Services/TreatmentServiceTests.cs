using System.Linq.Expressions;
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Dme.Application.DTOs.Treatments;
using Dme.Domain.Models;
using Dme.Infrastructure.Implementation;
using Dme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Dme.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the TreatmentService class.
/// </summary>
public class TreatmentServiceTests
{
    private readonly Mock<IRepository<Treatments, DmeDbContext>> _mockRepositoryTreatment;
    private readonly Mock<IRepository<Consultations, DmeDbContext>> _mockRepositoryConsultation;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TreatmentService _service;

    /// <summary>
    /// Initializes a new instance of the TreatmentServiceTests class.
    /// </summary>
    public TreatmentServiceTests()
    {
        _mockRepositoryTreatment = new Mock<IRepository<Treatments, DmeDbContext>>();
        _mockRepositoryConsultation = new Mock<IRepository<Consultations, DmeDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _service = new TreatmentService(_mockRepositoryTreatment.Object, _mockRepositoryConsultation.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Tests the GetTreatmentForConsultationById method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetTreatmentForConsultationById_ValidId_ReturnsTreatmentsReadDto()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        var treatment = new Treatments { TreatmentsId = idTreatment, FkIdConsultation = idConsultation };
        var expectedDto = new TreatmentsReadDto();
        _mockRepositoryTreatment.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<Treatments, bool>>>(expr => expr.Compile()(treatment)),
            It.IsAny<Func<IQueryable<Treatments>, IIncludableQueryable<Treatments, object>>>()))
            .ReturnsAsync(treatment);
        _mockMapper.Setup(m => m.Map<TreatmentsReadDto>(treatment)).Returns(expectedDto);

        // Act
        var result = await _service.GetTreatmentForConsultationById(idConsultation, idTreatment);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetTreatmentForConsultationById method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetTreatmentForConsultationById_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetTreatmentForConsultationById(invalidId, invalidId));
    }

    /// <summary>
    /// Tests the GetAllTreatmentForConsultationById method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetAllTreatmentForConsultationById_ValidId_ReturnsListOfTreatmentsReadDto()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var treatments = new List<Treatments> { new Treatments() };
        var expectedDtos = new List<TreatmentsReadDto> { new TreatmentsReadDto() };
        _mockRepositoryTreatment.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<Treatments, bool>>>(),
            It.IsAny<Func<IQueryable<Treatments>, IIncludableQueryable<Treatments, object>>>()))
            .ReturnsAsync(treatments);
        _mockMapper.Setup(m => m.Map<IEnumerable<TreatmentsReadDto>>(treatments)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllTreatmentForConsultationById(idConsultation);

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the CreateTreatmentForConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateTreatmentForConsultation_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var createDto = new TreatmentsCreateDto { ConsultationId = idConsultation };
        var treatment = new Treatments();

        _mockMapper.Setup(m => m.Map<Treatments>(createDto)).Returns(treatment);
        _mockRepositoryConsultation.Setup(repo => repo.GetAsync(a => a.ConsultationId == idConsultation,null))
            .ReturnsAsync(new Consultations { ConsultationId = idConsultation });
        _mockRepositoryTreatment.Setup(repo => repo.AddAsync(treatment)).Returns(Task.CompletedTask);
        _mockRepositoryTreatment.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateTreatmentForConsultation(idConsultation, createDto);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryTreatment.Verify(u => u.AddAsync(treatment), Times.Once);
        _mockRepositoryTreatment.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the CreateTreatmentForConsultation method with invalid data.
    /// </summary>
    [Fact]
    public async Task CreateTreatmentForConsultation_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new TreatmentsCreateDto(); // Invalid, IdDme is null

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateTreatmentForConsultation(Guid.NewGuid(), createDto));
    }

    /// <summary>
    /// Tests the PatchTreatmentForConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchTreatmentForConsultation_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        var patchDto = new TreatmentsPatchDto();
        var treatment = new Treatments { TreatmentsId = idTreatment, FkIdConsultation = idConsultation };

        _mockRepositoryTreatment.Setup(u => u.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation,null))
            .ReturnsAsync(treatment);
        _mockMapper.Setup(m => m.Map<TreatmentsPatchDto>(treatment)).Returns(patchDto);
        patchDto.UpdateWithDto(treatment);
        _mockRepositoryTreatment.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.PatchTreatmentForConsultation(idConsultation, idTreatment, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryTreatment.Verify(u => u.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation, null), Times.Once);
        _mockRepositoryTreatment.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the PatchTreatmentForConsultation method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task PatchTreatmentForConsultation_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var patchDto = new TreatmentsPatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.PatchTreatmentForConsultation(invalidId, invalidId, patchDto));
    }

    /// <summary>
    /// Tests the DeleteTreatmentForConsultation method with a valid ID.
    /// </summary>
    [Fact]
    public async Task DeleteTreatmentForConsultation_ValidId_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        var treatment = new Treatments { TreatmentsId = idTreatment, FkIdConsultation = idConsultation };

        _mockRepositoryTreatment.Setup(u => u.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation,null))
            .ReturnsAsync(treatment);
        _mockRepositoryTreatment.Setup(repo => repo.Remove(treatment));
        _mockRepositoryTreatment.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteTreatmentForConsultation(idConsultation, idTreatment);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryTreatment.Verify(u => u.Remove(treatment), Times.Once);
        _mockRepositoryTreatment.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the DeleteTreatmentForConsultation method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task DeleteTreatmentForConsultation_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteTreatmentForConsultation(invalidId, invalidId));
    }
}
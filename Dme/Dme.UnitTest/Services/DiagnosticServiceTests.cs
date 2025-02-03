using System.Linq.Expressions;
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Dme.Application.DTOs.Diagnostics;
using Dme.Domain.Models;
using Dme.Infrastructure.Implementation;
using Dme.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Dme.UnitTest.Services;

public class DiagnosticServiceTests
{
    private readonly Mock<IRepository<Diagnostics, DmeDbContext>> _mockRepositoryDiagnostic;
    private readonly Mock<IRepository<Consultations, DmeDbContext>> _mockRepositoryConsultation;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DiagnosticService _service;

    /// <summary>
    /// Initializes a new instance of the DiagnosticServiceTests class.
    /// </summary>
    public DiagnosticServiceTests()
    {
        _mockRepositoryDiagnostic = new Mock<IRepository<Diagnostics, DmeDbContext>>();
        _mockRepositoryConsultation = new Mock<IRepository<Consultations, DmeDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _service = new DiagnosticService(_mockRepositoryDiagnostic.Object, _mockRepositoryConsultation.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Tests the GetDiagnosticForConsultation method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetDiagnosticForConsultation_ValidId_ReturnsDiagnosticsReadDto()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        var diagnostic = new Diagnostics { DiagnosticId = idDiagnostic, FkIdConsultation = idConsultation };
        var expectedDto = new DiagnosticsReadDto();
        _mockRepositoryDiagnostic.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<Diagnostics, bool>>>(expr => expr.Compile()(diagnostic)),
            It.IsAny<Func<IQueryable<Diagnostics>, IIncludableQueryable<Diagnostics, object>>>()))
            .ReturnsAsync(diagnostic);
        _mockMapper.Setup(m => m.Map<DiagnosticsReadDto>(diagnostic)).Returns(expectedDto);

        // Act
        var result = await _service.GetDiagnosticForConsultation(idConsultation, idDiagnostic);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetDiagnosticForConsultation method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetDiagnosticForConsultation_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetDiagnosticForConsultation(invalidId, invalidId));
    }

    /// <summary>
    /// Tests the GetAllDiagnosticForConsultation method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetAllDiagnosticForConsultation_ValidId_ReturnsListOfDiagnosticsReadDto()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var diagnostics = new List<Diagnostics> { new Diagnostics() };
        var expectedDtos = new List<DiagnosticsReadDto> { new DiagnosticsReadDto() };
        _mockRepositoryDiagnostic.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<Diagnostics, bool>>>(),
            It.IsAny<Func<IQueryable<Diagnostics>, IIncludableQueryable<Diagnostics, object>>>()))
            .ReturnsAsync(diagnostics);
        _mockMapper.Setup(m => m.Map<IEnumerable<DiagnosticsReadDto>>(diagnostics)).Returns(expectedDtos);

        // Act
        var result = await _service.GetAllDiagnosticForConsultation(idConsultation);

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the CreateDiagnosticForConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateDiagnosticForConsultation_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var createDto = new DiagnosticsCreateDto { ConsultationId = idConsultation };
        var diagnostic = new Diagnostics();

        _mockMapper.Setup(m => m.Map<Diagnostics>(createDto)).Returns(diagnostic);
        _mockRepositoryConsultation.Setup(repo => repo.GetAsync(a => a.ConsultationId == idConsultation, null))
            .ReturnsAsync(new Consultations { ConsultationId = idConsultation });
        _mockRepositoryDiagnostic.Setup(repo => repo.AddAsync(diagnostic)).Returns(Task.CompletedTask);
        _mockRepositoryDiagnostic.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.CreateDiagnosticForConsultation(idConsultation, createDto);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryDiagnostic.Verify(u => u.AddAsync(diagnostic), Times.Once);
        _mockRepositoryDiagnostic.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the CreateDiagnosticForConsultation method with invalid data.
    /// </summary>
    [Fact]
    public async Task CreateDiagnosticForConsultation_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new DiagnosticsCreateDto(); // Invalid, IdDme is null

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateDiagnosticForConsultation(Guid.NewGuid(), createDto));
    }

    /// <summary>
    /// Tests the PatchDiagnosticForConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchDiagnosticForConsultation_ValidData_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        var patchDto = new DiagnosticsPatchDto();
        var diagnostic = new Diagnostics { DiagnosticId = idDiagnostic, FkIdConsultation = idConsultation };

        _mockRepositoryDiagnostic.Setup(u => u.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation,null))
            .ReturnsAsync(diagnostic);
        _mockMapper.Setup(m => m.Map<DiagnosticsPatchDto>(diagnostic)).Returns(patchDto);
        patchDto.UpdateWithDto(diagnostic);
        _mockRepositoryDiagnostic.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.PatchDiagnosticForConsultation(idConsultation, idDiagnostic, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryDiagnostic.Verify(u => u.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation, null), Times.Once);
        _mockRepositoryDiagnostic.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the PatchDiagnosticForConsultation method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task PatchDiagnosticForConsultation_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var patchDto = new DiagnosticsPatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.PatchDiagnosticForConsultation(invalidId, invalidId, patchDto));
    }

    /// <summary>
    /// Tests the DeleteDiagnosticForConsultation method with a valid ID.
    /// </summary>
    [Fact]
    public async Task DeleteDiagnosticForConsultation_ValidId_ReturnsSuccess()
    {
        // Arrange
        var idConsultation = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        var diagnostic = new Diagnostics { DiagnosticId = idDiagnostic, FkIdConsultation = idConsultation };

        _mockRepositoryDiagnostic.Setup(u => u.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation,null))
            .ReturnsAsync(diagnostic);
        _mockRepositoryDiagnostic.Setup(repo => repo.Remove(diagnostic));
        _mockRepositoryDiagnostic.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteDiagnosticForConsultation(idConsultation, idDiagnostic);

        // Assert
        Assert.True((bool)result);
        _mockRepositoryDiagnostic.Verify(u => u.Remove(diagnostic), Times.Once);
        _mockRepositoryDiagnostic.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the DeleteDiagnosticForConsultation method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task DeleteDiagnosticForConsultation_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteDiagnosticForConsultation(invalidId, invalidId));
    }
}
using System.Linq.Expressions;
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Patients.Application.DTOs.Adresse;
using Patients.Domain.Models;
using Patients.Infrastructure.Implementation;
using Patients.Infrastructure.Persistence;

namespace Patients.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the AdresseService class.
/// </summary>
public class AdresseServiceTests
{
    // Mocks for dependencies
    private readonly Mock<IRepository<Adresse, PatientDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AdresseService _service;

    /// <summary>
    /// Initializes a new instance of the AdresseServiceTests class.
    /// </summary>
    public AdresseServiceTests()
    {
        _mockRepository = new Mock<IRepository<Adresse, PatientDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _service = new AdresseService(_mockRepository.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Tests the GetAdresse method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetAdresse_ValidId_ReturnsAdresseDto()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var adresse = new Adresse { FkIdPatient = patientId, AdresseId = adresseId };
        var expectedDto = new AdresseDto();
        _mockRepository.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<Adresse, bool>>>(expr => expr.Compile()(adresse)),
            It.IsAny<Func<IQueryable<Adresse>, IIncludableQueryable<Adresse, object>>>()
        )).ReturnsAsync(adresse);
        _mockMapper.Setup(m => m.Map<AdresseDto>(adresse)).Returns(expectedDto);

        // Act
        var result = await _service.GetAdresse(patientId, adresseId);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetAdresse method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetAdresse_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var adresseId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetAdresse(patientId, adresseId));
    }

    /// <summary>
    /// Tests the GetRelative method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetRelative_ValidId_ReturnsListOfAdresseDto()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var adresses = new List<Adresse> { new() { FkIdPatient = patientId } };
        var expectedDtos = new List<AdresseDto> { new() };
        _mockRepository.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<Adresse, bool>>>(),
            It.IsAny<Func<IQueryable<Adresse>, IIncludableQueryable<Adresse, object>>>()
        )).ReturnsAsync(adresses);
        _mockMapper.Setup(m => m.Map<IEnumerable<AdresseDto>>(adresses)).Returns(expectedDtos);

        // Act
        var result = await _service.GetRelative(patientId);

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the GetRelative method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetRelative_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetRelative(patientId));
    }

    /// <summary>
    /// Tests the Create method with valid data.
    /// </summary>
    [Fact]
    public async Task Create_ValidData_ReturnsSuccess()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var createDto = new AdresseCreateDto();
        var adresse = new Adresse();
        _mockMapper.Setup(m => m.Map<Adresse>(createDto)).Returns(adresse);
        _mockRepository.Setup(repo => repo.AddAsync(adresse)).Returns(Task.CompletedTask);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Create(patientId, createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(true, result);
        _mockRepository.Verify(u => u.AddAsync(adresse), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the Create method with an invalid patient ID.
    /// </summary>
    [Fact]
    public async Task Create_InvalidPatientId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var createDto = new AdresseCreateDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Create(patientId, createDto));
    }

    /// <summary>
    /// Tests the Patch method with valid data.
    /// </summary>
    [Fact]
    public async Task Patch_ValidData_ReturnsSuccess()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var patchDto = new AdressePatchDto();
        var adresse = new Adresse { FkIdPatient = patientId, AdresseId = adresseId };

        _mockRepository.Setup(u => u.GetAsync(z => z.AdresseId == adresseId && z.FkIdPatient == patientId, null))
            .ReturnsAsync(adresse);
        _mockMapper.Setup(m => m.Map<AdressePatchDto>(adresse)).Returns(patchDto);
        patchDto.UpdateWithDto(adresse);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Patch(patientId, adresseId, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(z => z.AdresseId == adresseId && z.FkIdPatient == patientId, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
    }

    /// <summary>
    /// Tests the Patch method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Patch_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var adresseId = Guid.Empty;
        var patchDto = new AdressePatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Patch(patientId, adresseId, patchDto));
    }

    /// <summary>
    /// Tests the Delete method with a valid ID.
    /// </summary>
    [Fact]
    public async Task Delete_ValidId_ReturnsSuccess()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var adresseId = Guid.NewGuid();
        var adresse = new Adresse { FkIdPatient = patientId, AdresseId = adresseId };
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<Adresse, bool>>>(expr => expr.Compile()(adresse)),
            It.IsAny<Func<IQueryable<Adresse>, IIncludableQueryable<Adresse, object>>>()
        )).ReturnsAsync(adresse);
        _mockRepository.Setup(repo => repo.Remove(adresse));
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Delete(patientId, adresseId);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(adresse), Times.Once);
    }

    /// <summary>
    /// Tests the Delete method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Delete_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var adresseId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Delete(patientId, adresseId));
    }
}
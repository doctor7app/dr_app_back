using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Patients.Application.DTOs.MedicalInfo;
using Patients.Domain.Models;
using Patients.Infrastructure.Implementation;
using Patients.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Patients.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the MedicalInfoService class.
/// </summary>
public class MedicalInfoServiceTests
{
    // Mocks for dependencies
    private readonly Mock<IRepository<MedicalInformation, PatientDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MedicalInfoService _service;

    /// <summary>
    /// Initializes a new instance of the MedicalInfoServiceTests class.
    /// </summary>
    public MedicalInfoServiceTests()
    {
        _mockRepository = new Mock<IRepository<MedicalInformation, PatientDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _service = new MedicalInfoService(_mockRepository.Object, _mockMapper.Object);
    }

    /// <summary>
    /// Tests the GetMedicalInfo method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetMedicalInfo_ValidId_ReturnsMedicalInfoDto()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var medicalInfo = new MedicalInformation { FkIdPatient = patientId, MedicalInformationId = medicalInfoId };
        var expectedDto = new MedicalInfoDto();
        _mockRepository.Setup(repo => repo.GetAsync(
            It.Is<Expression<Func<MedicalInformation, bool>>>(expr => expr.Compile()(medicalInfo)),
            It.IsAny<Func<IQueryable<MedicalInformation>, IIncludableQueryable<MedicalInformation, object>>>()))
            .ReturnsAsync(medicalInfo);

        _mockMapper.Setup(m => m.Map<MedicalInfoDto>(medicalInfo)).Returns(expectedDto);

        // Act
        var result = await _service.GetMedicalInfo(patientId, medicalInfoId);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the GetMedicalInfo method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task GetMedicalInfo_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var medicalInfoId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.GetMedicalInfo(patientId, medicalInfoId));
    }

    /// <summary>
    /// Tests the GetRelative method with a valid ID.
    /// </summary>
    [Fact]
    public async Task GetRelative_ValidId_ReturnsListOfMedicalInfoDto()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var medicalInfos = new List<MedicalInformation> { new() { FkIdPatient = patientId } };
        var expectedDtos = new List<MedicalInfoDto> { new() };
        _mockRepository.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<MedicalInformation, bool>>>(),
            It.IsAny<Func<IQueryable<MedicalInformation>, IIncludableQueryable<MedicalInformation, object>>>()))
            .ReturnsAsync(medicalInfos);
        _mockMapper.Setup(m => m.Map<IEnumerable<MedicalInfoDto>>(medicalInfos)).Returns(expectedDtos);

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
        var createDto = new MedicalInfoCreateDto();
        var medicalInfo = new MedicalInformation();
        _mockMapper.Setup(m => m.Map<MedicalInformation>(createDto)).Returns(medicalInfo);
        _mockRepository.Setup(repo => repo.AddAsync(medicalInfo)).Returns(Task.CompletedTask);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);
        // Act
        var result = await _service.Create(patientId, createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(true, result);
        _mockRepository.Verify(u => u.AddAsync(medicalInfo), Times.Once);
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
        var createDto = new MedicalInfoCreateDto();

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
        var medicalInfoId = Guid.NewGuid();
        var patchDto = new MedicalInfoPatchDto();
        var medicalInfo = new MedicalInformation { FkIdPatient = patientId, MedicalInformationId = medicalInfoId };
        
        _mockRepository.Setup(u => u.GetAsync(z => z.MedicalInformationId == medicalInfoId
                                                   && z.FkIdPatient == patientId, null))
            .ReturnsAsync(medicalInfo);

        _mockMapper.Setup(m => m.Map<MedicalInfoPatchDto>(medicalInfo)).Returns(patchDto);
        patchDto.UpdateWithDto(medicalInfo);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Patch(patientId, medicalInfoId, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(z => z.MedicalInformationId == medicalInfoId
                                                    && z.FkIdPatient == patientId, null), Times.Once);
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
        var medicalInfoId = Guid.Empty;
        var patchDto = new MedicalInfoPatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Patch(patientId, medicalInfoId, patchDto));
    }

    /// <summary>
    /// Tests the Delete method with a valid ID.
    /// </summary>
    [Fact]
    public async Task Delete_ValidId_ReturnsSuccess()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var medicalInfoId = Guid.NewGuid();
        var medicalInfo = new MedicalInformation { FkIdPatient = patientId, MedicalInformationId = medicalInfoId };
        _mockRepository.Setup(u => u.GetAsync(
            It.Is<Expression<Func<MedicalInformation, bool>>>(expr => expr.Compile()(medicalInfo)),
            It.IsAny<Func<IQueryable<MedicalInformation>, IIncludableQueryable<MedicalInformation, object>>>()
        )).ReturnsAsync(medicalInfo);
        _mockRepository.Setup(repo => repo.Remove(medicalInfo));
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Delete(patientId, medicalInfoId);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(medicalInfo), Times.Once);
    }

    /// <summary>
    /// Tests the Delete method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Delete_InvalidId_ThrowsException()
    {
        // Arrange
        var patientId = Guid.Empty;
        var medicalInfoId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Delete(patientId, medicalInfoId));
    }
}
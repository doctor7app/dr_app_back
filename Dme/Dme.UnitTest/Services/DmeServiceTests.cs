using System.Linq.Expressions;
using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Contracts.Messages.Dmes;
using Contracts.Messages.Patients;
using Dme.Application.DTOs.Dmes;
using Dme.Infrastructure.Implementation;
using Dme.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Dme.UnitTest.Services;

/// <summary>
/// This class contains unit tests for the DmeService class.
/// </summary>
public class DmeServiceTests
{
    private readonly Mock<IRepository<Domain.Models.Dme, DmeDbContext>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly DmeService _service;

    /// <summary>
    /// Initializes a new instance of the DmeServiceTests class.
    /// </summary>
    public DmeServiceTests()
    {
        _mockRepository = new Mock<IRepository<Domain.Models.Dme, DmeDbContext>>();
        _mockMapper = new Mock<IMapper>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _service = new DmeService(_mockRepository.Object, _mockMapper.Object, _mockPublishEndpoint.Object);
    }

    /// <summary>
    /// Tests the Get method with a valid ID.
    /// </summary>
    [Fact]
    public async Task Get_ValidId_ReturnsDmeReadDto()
    {
        // Arrange
        var dmeId = Guid.NewGuid();
        var dme = new Domain.Models.Dme { DmeId = dmeId };
        var expectedDto = new DmeReadDto();
        _mockRepository.Setup(repo => repo.GetAsync(
                It.Is<Expression<Func<Domain.Models.Dme, bool>>>(expr => expr.Compile()(dme)),
                It.Is<Func<IQueryable<Domain.Models.Dme>, IIncludableQueryable<Domain.Models.Dme, object>>>(includes => true)))
            .ReturnsAsync(dme);
        _mockMapper.Setup(m => m.Map<DmeReadDto>(dme)).Returns(expectedDto);

        // Act
        var result = await _service.Get(dmeId);

        // Assert
        Assert.Equal(expectedDto, result);
    }

    /// <summary>
    /// Tests the Get method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Get_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Get(invalidId));
    }

    /// <summary>
    /// Tests the Get method to retrieve all DME records.
    /// </summary>
    [Fact]
    public async Task GetAll_ReturnsListOfDmeReadDto()
    {
        // Arrange
        var dmes = new List<Domain.Models.Dme> { new Domain.Models.Dme() };
        var expectedDtos = new List<DmeReadDto> { new DmeReadDto() };
        _mockRepository.Setup(repo => repo.GetListAsync(
            It.IsAny<Expression<Func<Domain.Models.Dme, bool>>>(),
            It.IsAny<Func<IQueryable<Domain.Models.Dme>, IIncludableQueryable<Domain.Models.Dme, object>>>()
            )).ReturnsAsync(dmes);
        _mockMapper.Setup(m => m.Map<IEnumerable<DmeReadDto>>(dmes)).Returns(expectedDtos);

        // Act
        var result = await _service.Get();

        // Assert
        Assert.Equal(expectedDtos, result);
    }

    /// <summary>
    /// Tests the Create method with valid data.
    /// </summary>
    [Fact]
    public async Task Create_ValidData_ReturnsSuccess()
    {
        // Arrange
        var createDto = new DmeCreateDto { DoctorId = Guid.NewGuid() };
        var dme = new Domain.Models.Dme();
        var entityCreatedEvent = new DmeCreatedEvent();

        _mockMapper.Setup(m => m.Map<DmeCreatedEvent>(It.IsAny<DmeCreateDto>())).Returns(entityCreatedEvent);
        _mockMapper.Setup(m => m.Map<Domain.Models.Dme>(createDto)).Returns(dme);
        _mockRepository.Setup(repo => repo.AddAsync(dme)).Returns(Task.CompletedTask);
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Create(createDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.AddAsync(dme), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(entityCreatedEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests the Create method with invalid data.
    /// </summary>
    [Fact]
    public async Task Create_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new DmeCreateDto(); // Invalid, DoctorId is null

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Create(createDto));
    }

    /// <summary>
    /// Tests the Patch method with valid data.
    /// </summary>
    [Fact]
    public async Task Patch_ValidData_ReturnsSuccess()
    {
        // Arrange
        var dmeId = Guid.NewGuid();
        var patchDto = new DmePatchDto();
        var dme = new Domain.Models.Dme { DmeId = dmeId };
        var updatedEvent = new DmeUpdatedEvent
        {
            DoctorId = Guid.NewGuid(),
            PatientId = Guid.NewGuid()
        };

        _mockRepository.Setup(u => u.GetAsync(x => x.DmeId == dmeId,null))
            .ReturnsAsync(dme);
        _mockMapper.Setup(m => m.Map<DmePatchDto>(dme)).Returns(patchDto);
        _mockMapper.Setup(m => m.Map<DmeUpdatedEvent>(patchDto)).Returns(updatedEvent);
        patchDto.UpdateWithDto(dme); 
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Patch(dmeId, patchDto);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.GetAsync(x => x.DmeId == dmeId, null), Times.Once);
        _mockRepository.Verify(u => u.Complete(), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(updatedEvent, It.IsAny<CancellationToken>()), Times.Once);

    }

    /// <summary>
    /// Tests the Patch method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Patch_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;
        var patchDto = new DmePatchDto();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Patch(invalidId, patchDto));
    }

    /// <summary>
    /// Tests the Delete method with a valid ID.
    /// </summary>
    [Fact]
    public async Task Delete_ValidId_ReturnsSuccess()
    {
        // Arrange
        var dmeId = Guid.NewGuid();
        var dme = new Domain.Models.Dme { DmeId = dmeId };
        _mockRepository.Setup(u => u.GetAsync(x => x.DmeId == dmeId, null))
            .ReturnsAsync(dme);
        _mockRepository.Setup(repo => repo.Remove(dme));
        _mockRepository.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _service.Delete(dmeId);

        // Assert
        Assert.True((bool)result);
        _mockRepository.Verify(u => u.Remove(dme), Times.Once);
        _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<DmeDeletedEvent>(), default), Times.Once);
    }

    /// <summary>
    /// Tests the Delete method with an invalid ID.
    /// </summary>
    [Fact]
    public async Task Delete_InvalidId_ThrowsException()
    {
        // Arrange
        var invalidId = Guid.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.Delete(invalidId));
    }
}
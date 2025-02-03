using AutoMapper;
using Dme.Api.Controllers;
using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Dme.UnitTest.Controllers;

public class DmesControllerTests
{
    private readonly Mock<IDmeService> _mockDmeService;
    private readonly Mock<IConsultationService> _mockConsultationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DmesController _controller;

    /// <summary>
    /// Initializes a new instance of the DmesControllerTests class.
    /// </summary>
    public DmesControllerTests()
    {
        _mockDmeService = new Mock<IDmeService>();
        _mockConsultationService = new Mock<IConsultationService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new DmesController(_mockDmeService.Object, _mockConsultationService.Object);
    }

    /// <summary>
    /// Tests the Get method for a valid key.
    /// </summary>
    [Fact]
    public async Task Get_ValidKey_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var expectedDme = new DmeReadDto();
        _mockDmeService.Setup(service => service.Get(key)).ReturnsAsync(expectedDme);

        // Act
        var result = await _controller.Get(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDme, okResult.Value);
    }

    /// <summary>
    /// Tests the Get method for all DMEs.
    /// </summary>
    [Fact]
    public async Task Get_AllDmes_ReturnsOkResult()
    {
        // Arrange
        var expectedDmes = new List<DmeReadDto> { new DmeReadDto() };
        _mockDmeService.Setup(service => service.Get()).ReturnsAsync(expectedDmes);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDmes, okResult.Value);
    }

    /// <summary>
    /// Tests the Post method with valid data.
    /// </summary>
    [Fact]
    public async Task Post_ValidData_ReturnsOkResult()
    {
        // Arrange
        var createDto = new DmeCreateDto();
        _mockMapper.Setup(m => m.Map<DmeCreateDto>(createDto)).Returns(createDto);
        _mockDmeService.Setup(service => service.Create(createDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.Post(createDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the Post method with invalid data.
    /// </summary>
    [Fact]
    public async Task Post_InvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new DmeCreateDto(); // Invalid data

        _controller.ModelState.AddModelError("PatientId", "First Name is Required");
        _controller.ModelState.AddModelError("DoctorId", "Last Name is Required");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Post(createDto));
    }

    /// <summary>
    /// Tests the Patch method with valid data.
    /// </summary>
    [Fact]
    public async Task Patch_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var patchDto = new DmePatchDto();
        _mockDmeService.Setup(service => service.Patch(key, patchDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.Patch(key, patchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the Patch method with invalid data.
    /// </summary>
    [Fact]
    public async Task Patch_InvalidData_ThrowsException()
    {
        // Arrange
        var key = Guid.NewGuid();
        var patchDto = new DmePatchDto(); // Invalid data

        _mockDmeService.Setup(x => x.Patch(key, patchDto)).ReturnsAsync(false);
        _controller.ModelState.AddModelError("PatientId", "First Name is Required");
        _controller.ModelState.AddModelError("DoctorId", "Last Name is Required");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Patch(key, patchDto));
    }

    /// <summary>
    /// Tests the Delete method with a valid key.
    /// </summary>
    [Fact]
    public async Task Delete_ValidKey_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        _mockDmeService.Setup(service => service.Delete(key)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the GetConsultation method with valid IDs.
    /// </summary>
    [Fact]
    public async Task GetConsultation_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        var expectedConsultation = new ConsultationsReadDto();
        _mockConsultationService.Setup(service => service.GetConsultationForDme(key, idConsultation)).ReturnsAsync(expectedConsultation);

        // Act
        var result = await _controller.GetConsultation(key, idConsultation);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedConsultation, okResult.Value);
    }

    /// <summary>
    /// Tests the CreateConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateConsultation_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var createDto = new ConsultationsCreateDto();
        _mockConsultationService.Setup(service => service.CreateConsultationForDme(key, createDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateConsultation(key, createDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the PatchConsultation method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchConsultation_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        var patchDto = new ConsultationsPatchDto();
        _mockConsultationService.Setup(service => service.PatchConsultationForDme(key, idConsultation, patchDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.PatchConsultation(key, idConsultation, patchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the DeleteConsultation method with valid IDs.
    /// </summary>
    [Fact]
    public async Task DeleteConsultation_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idConsultation = Guid.NewGuid();
        _mockConsultationService.Setup(service => service.DeleteConsultationForDme(key, idConsultation)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteConsultation(key, idConsultation);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }
}
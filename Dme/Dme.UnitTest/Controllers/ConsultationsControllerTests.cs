using Dme.Api.Controllers;
using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Diagnostics;
using Dme.Application.DTOs.Treatments;
using Dme.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Dme.UnitTest.Controllers;

/// <summary>
/// This class contains unit tests for the ConsultationsController class.
/// </summary>
public class ConsultationsControllerTests
{
    private readonly Mock<IConsultationService> _mockConsultationService;
    private readonly Mock<ITreatmentService> _mockTreatmentService;
    private readonly Mock<IDiagnosticService> _mockDiagnosticService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ConsultationsController _controller;

    /// <summary>
    /// Initializes a new instance of the ConsultationsControllerTests class.
    /// </summary>
    public ConsultationsControllerTests()
    {
        _mockConsultationService = new Mock<IConsultationService>();
        _mockTreatmentService = new Mock<ITreatmentService>();
        _mockDiagnosticService = new Mock<IDiagnosticService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ConsultationsController(_mockConsultationService.Object, _mockTreatmentService.Object,
            _mockDiagnosticService.Object);
    }

    /// <summary>
    /// Tests the Get method for a valid key.
    /// </summary>
    [Fact]
    public async Task Get_ValidKey_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var expectedConsultation = new ConsultationsReadDto();
        _mockConsultationService.Setup(service => service.GetConsultationById(key)).ReturnsAsync(expectedConsultation);

        // Act
        var result = await _controller.Get(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedConsultation, okResult.Value);
    }

    /// <summary>
    /// Tests the Get method for all consultations.
    /// </summary>
    [Fact]
    public async Task Get_AllConsultations_ReturnsOkResult()
    {
        // Arrange
        var expectedConsultations = new List<ConsultationsReadDto> { new ConsultationsReadDto() };
        _mockConsultationService.Setup(service => service.GetAllConsultation()).ReturnsAsync(expectedConsultations);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedConsultations, okResult.Value);
    }

    /// <summary>
    /// Tests the Post method with valid data.
    /// </summary>
    [Fact]
    public async Task Post_ValidData_ReturnsOkResult()
    {
        // Arrange
        var createDto = new ConsultationsCreateDto();
        _mockMapper.Setup(m => m.Map<ConsultationsCreateDto>(createDto)).Returns(createDto);
        _mockConsultationService.Setup(service => service.CreateConsultation(createDto)).ReturnsAsync(true);

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
        var createDto = new ConsultationsCreateDto();
        _controller.ModelState.AddModelError("ConsultationDate", "The appointment date cannot be in the past.");

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
        var patchDto = new ConsultationsPatchDto();
        _mockConsultationService.Setup(service => service.PatchConsultationById(key, patchDto)).ReturnsAsync(true);

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
        var patchDto = new ConsultationsPatchDto(); // Invalid data

        _controller.ModelState.AddModelError("Weight", "Weight must be a positive value.");
        _controller.ModelState.AddModelError("ConsultationDate", "The appointment date cannot be in the past.");

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
        _mockConsultationService.Setup(service => service.DeleteConsultationById(key)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the GetTreatment method with valid IDs.
    /// </summary>
    [Fact]
    public async Task GetTreatment_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        var expectedTreatment = new TreatmentsReadDto();
        _mockTreatmentService.Setup(service => service.GetTreatmentForConsultationById(key, idTreatment))
            .ReturnsAsync(expectedTreatment);

        // Act
        var result = await _controller.GetTreatment(key, idTreatment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedTreatment, okResult.Value);
    }

    /// <summary>
    /// Tests the CreateTreatment method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateTreatment_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var createDto = new TreatmentsCreateDto();
        _mockTreatmentService.Setup(service => service.CreateTreatmentForConsultation(key, createDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateTreatment(key, createDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the PatchTreatment method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchTreatment_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        var patchDto = new TreatmentsPatchDto();
        _mockTreatmentService.Setup(service => service.PatchTreatmentForConsultation(key, idTreatment, patchDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.PatchTreatment(key, idTreatment, patchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the DeleteTreatment method with valid IDs.
    /// </summary>
    [Fact]
    public async Task DeleteTreatment_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idTreatment = Guid.NewGuid();
        _mockTreatmentService.Setup(service => service.DeleteTreatmentForConsultation(key, idTreatment))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTreatment(key, idTreatment);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the GetDiagnostic method with valid IDs.
    /// </summary>
    [Fact]
    public async Task GetDiagnostic_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        var expectedDiagnostic = new DiagnosticsReadDto();
        _mockDiagnosticService.Setup(service => service.GetDiagnosticForConsultation(key, idDiagnostic))
            .ReturnsAsync(expectedDiagnostic);

        // Act
        var result = await _controller.GetDiagnostic(key, idDiagnostic);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDiagnostic, okResult.Value);
    }

    /// <summary>
    /// Tests the CreateDiagnostic method with valid data.
    /// </summary>
    [Fact]
    public async Task CreateDiagnostic_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var createDto = new DiagnosticsCreateDto();
        _mockDiagnosticService.Setup(service => service.CreateDiagnosticForConsultation(key, createDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CreateDiagnostic(key, createDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the PatchDiagnostic method with valid data.
    /// </summary>
    [Fact]
    public async Task PatchDiagnostic_ValidData_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        var patchDto = new DiagnosticsPatchDto();
        _mockDiagnosticService.Setup(service => service.PatchDiagnosticForConsultation(key, idDiagnostic, patchDto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.PatchDiagnostic(key, idDiagnostic, patchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    /// <summary>
    /// Tests the DeleteDiagnostic method with valid IDs.
    /// </summary>
    [Fact]
    public async Task DeleteDiagnostic_ValidIds_ReturnsOkResult()
    {
        // Arrange
        var key = Guid.NewGuid();
        var idDiagnostic = Guid.NewGuid();
        _mockDiagnosticService.Setup(service => service.DeleteDiagnosticForConsultation(key, idDiagnostic))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDiagnostic(key, idDiagnostic);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }
}

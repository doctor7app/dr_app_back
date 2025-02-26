using FluentAssertions;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prescriptions.Api.Controllers;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces.Services;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Results;

namespace Prescriptions.UnitTest.Controllers;

public class PrescriptionControllerTest
{
    private readonly Mock<IPrescriptionService> _prescriptionServiceMock = new();
    private readonly Mock<IPrescriptionItemService> _prescriptionItemServiceMock = new();
    private readonly Mock<IValidator<PrescriptionCreateDto>> _prescriptionCreateValidatorMock = new();
    private readonly Mock<IValidator<PrescriptionUpdateDto>> _prescriptionUpdateValidatorMock = new();
    private readonly Mock<IValidator<PrescriptionItemCreateDto>> _prescriptionItemCreateValidatorMock = new();
    private readonly Mock<IValidator<PrescriptionItemUpdateDto>> _prescriptionItemUpdateValidatorMock = new();

    private readonly PrescriptionsController _controller;

    public PrescriptionControllerTest()
    {
        _controller = new PrescriptionsController(
            _prescriptionServiceMock.Object,
            _prescriptionItemServiceMock.Object,
            _prescriptionCreateValidatorMock.Object,
            _prescriptionUpdateValidatorMock.Object,
            _prescriptionItemCreateValidatorMock.Object,
            _prescriptionItemUpdateValidatorMock.Object
        );
    }

    [Fact]
    public async Task Get_ReturnsAllPrescriptions()
    {
        // Arrange
        var prescriptions = new List<PrescriptionDto> { new PrescriptionDto { Id = Guid.NewGuid() } };
        _prescriptionServiceMock.Setup(service => service.GetAllPrescriptionAsync())
            .ReturnsAsync(prescriptions);

        // Act
        var result = await _controller.Get();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeAssignableTo<IQueryable<PrescriptionDto>>();
    }

    [Fact]
    public async Task Get_WithId_ReturnsPrescription_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var prescription = new PrescriptionDto { Id = id };
        _prescriptionServiceMock.Setup(service => service.GetPrescriptionByIdAsync(id))
            .ReturnsAsync(prescription);

        // Act
        var result = await _controller.Get(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().Be(prescription);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var dto = new PrescriptionCreateDto();
        _prescriptionCreateValidatorMock.Setup(validator => validator.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Field", "Error") }));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsOk_WithCreatedPrescription()
    {
        // Arrange
        var dto = new PrescriptionCreateDto();

        _prescriptionCreateValidatorMock.Setup(validator => validator.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _prescriptionServiceMock.Setup(service => service.CreatePrescriptionAsync(dto))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        var id = Guid.NewGuid();
        _prescriptionServiceMock.Setup(service => service.DeletePrescriptionAsync(id))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPrescriptionNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _prescriptionServiceMock.Setup(service => service.DeletePrescriptionAsync(id))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #region Items


    [Fact]
    public async Task GetPrescriptionItem_ReturnsOk_WhenItemExists()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var item = new PrescriptionItemDto { Id = itemId };

        _prescriptionItemServiceMock.Setup(s => s.GetItemByIdAsync(key, itemId)).ReturnsAsync(item);

        var result = await _controller.GetPrescriptionItem(key, itemId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(item, okResult.Value);
    }

    [Fact]
    public async Task GetPrescriptionItem_ReturnsNotFound_WhenItemDoesNotExist()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _prescriptionItemServiceMock.Setup(s => s.GetItemByIdAsync(key, itemId)).ReturnsAsync((PrescriptionItemDto)null!);

        var result = await _controller.GetPrescriptionItem(key, itemId);
        
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CreatePrescriptionItem_ReturnsOk_WhenValid()
    {
        var key = Guid.NewGuid();
        var createDto = new PrescriptionItemCreateDto();

        _prescriptionItemCreateValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());
        _prescriptionItemServiceMock.Setup(s => s.CreatePrescriptionItem(key, createDto)).ReturnsAsync(true);

        var result = await _controller.CreatePrescriptionItem(key, createDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, okResult.Value);
    }

    [Fact]
    public async Task CreatePrescriptionItem_ReturnsBadRequest_WhenValidationFails()
    {
        var key = Guid.NewGuid();
        var createDto = new PrescriptionItemCreateDto();
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Field", "Error") });

        _prescriptionItemCreateValidatorMock.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(validationResult);

        var result = await _controller.CreatePrescriptionItem(key, createDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(validationResult.Errors, badRequestResult.Value);
    }

    [Fact]
    public async Task PatchPrescriptionItem_ReturnsOk_WhenValid()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var patchDto = new Delta<PrescriptionItemUpdateDto>();

        _prescriptionItemUpdateValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<PrescriptionItemUpdateDto>(), default))
            .ReturnsAsync(new ValidationResult());
        _prescriptionItemServiceMock.Setup(s => s.UpdateItemAsync(key, itemId, patchDto)).ReturnsAsync(true);

        var result = await _controller.PatchPrescriptionItem(key, itemId, patchDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, okResult.Value);
    }

    [Fact]
    public async Task PatchPrescriptionItem_ReturnsBadRequest_WhenInvalidPatchObject()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var result = await _controller.PatchPrescriptionItem(key, itemId, null);

        var badRequestResult = Assert.IsType<BadRequestODataResult>(result);
        Assert.Equal("Invalid patch object.", badRequestResult.Error.Message.ToString());
    }

    [Fact]
    public async Task DeletePrescriptionItem_ReturnsOk_WhenDeleted()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _prescriptionItemServiceMock.Setup(s => s.DeleteItemAsync(key, itemId)).ReturnsAsync(true);

        var result = await _controller.DeletePrescriptionItem(key, itemId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeletePrescriptionItem_ReturnsNotFound_WhenNotExists()
    {
        var key = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _prescriptionItemServiceMock.Setup(s => s.DeleteItemAsync(key, itemId)).ReturnsAsync(false);

        var result = await _controller.DeletePrescriptionItem(key, itemId);

        Assert.IsType<OkObjectResult>(result);
    }

    #endregion
}
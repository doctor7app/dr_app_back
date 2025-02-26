using System.Linq.Expressions;
using AutoMapper;
using Common.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Application.Interfaces.Services;
using Prescriptions.Domain.Interfaces;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Implementation.Services;
using Prescriptions.Infrastructure.Persistence;


namespace Prescriptions.UnitTest.Services
{
    public class PrescriptionServiceTests
    {
        private readonly Mock<IRepository<Prescription, PrescriptionDbContext>> _mockRepo;
        private readonly Mock<IEventStoreService> _mockEventStore;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPublishEndpoint> _mockPublisher;
        private readonly PrescriptionService _service;

        public PrescriptionServiceTests()
        {
            _mockRepo = new Mock<IRepository<Prescription, PrescriptionDbContext>>();
            _mockEventStore = new Mock<IEventStoreService>();
            _mockMapper = new Mock<IMapper>();
            _mockPublisher = new Mock<IPublishEndpoint>();

            _service = new PrescriptionService(
                _mockRepo.Object,
                _mockEventStore.Object,
                _mockMapper.Object,
                _mockPublisher.Object
            );
        }

        [Fact]
        public async Task GetAllPrescriptionAsync_ReturnsMappedDtos()
        {
            // Arrange
            var prescriptions = new List<Prescription> { new() };
            var dtos = new List<PrescriptionDto> { new() };

            _mockRepo.Setup(repo => repo.GetListAsync(
                    It.IsAny<Expression<Func<Prescription, bool>>>(),
                    It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()))
                .ReturnsAsync(prescriptions);

            _mockMapper.Setup(m => m.Map<IEnumerable<PrescriptionDto>>(prescriptions))
                .Returns(dtos);

            // Act
            var result = await _service.GetAllPrescriptionAsync();

            // Assert
            Assert.Single(result);
            _mockRepo.Verify(r => r.GetListAsync(
                It.IsAny<Expression<Func<Prescription, bool>>>(),
                It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()), Times.Once);
        }

        [Fact]
        public async Task GetPrescriptionByIdAsync_InvalidId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetPrescriptionByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetPrescriptionByIdAsync_ValidId_ReturnsMappedDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            var prescription = new Prescription { PrescriptionId = id };
            var dto = new PrescriptionDto { Id = id };

            _mockRepo.Setup(repo => repo.GetAsync(
                    It.Is<Expression<Func<Prescription, bool>>>(expr => expr.Compile()(prescription)),
                    It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()))
                .ReturnsAsync(prescription);
            
            _mockMapper.Setup(m => m.Map<PrescriptionDto>(prescription))
                .Returns(dto);

            // Act
            var result = await _service.GetPrescriptionByIdAsync(id);

            // Assert
            Assert.Equal(id, result.Id);
            _mockRepo.Verify(r => r.GetAsync(
                It.Is<Expression<Func<Prescription, bool>>>(expr => expr.Compile()(prescription)),
                It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()), Times.Once);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_InvalidGuids_ThrowsException()
        {
            // Arrange
            var invalidDto = new PrescriptionCreateDto
            {
                DoctorId = Guid.Empty,
                PatientId = Guid.Empty,
                ConsultationId = Guid.Empty
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(invalidDto));
        }

        [Fact]
        public async Task CreatePrescriptionAsync_ValidDto_CreatesAndSavesPrescription()
        {
            // Arrange
            var validDto = new PrescriptionCreateDto
            {
                DoctorId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Items = { new PrescriptionItemCreateDto() }
            };
            var prescription = new Prescription
            {
                FkDoctorId = validDto.DoctorId,
                FkPatientId = validDto.PatientId,
                FkConsultationId = validDto.ConsultationId,
            };
            _mockMapper.Setup(m => m.Map<Prescription>(validDto)).Returns(prescription);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Prescription>()));
            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.CreatePrescriptionAsync(validDto);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Prescription>()), Times.Once);
            _mockRepo.Verify(r => r.Complete(), Times.Once);
            _mockEventStore.Verify(e => e.SaveEvents(It.IsAny<IReadOnlyCollection<IPrescriptionEvent>>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_InvalidId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdatePrescriptionAsync(Guid.Empty, It.IsAny<Delta<PrescriptionUpdateDto>>()));
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_NonExistingId_ThrowsException()
        {
            // Arrange
            var invalidId = Guid.Empty;
            
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.UpdatePrescriptionAsync(invalidId, new Delta<PrescriptionUpdateDto>()));
        }

        [Fact]
        public async Task UpdatePrescriptionAsync_ValidUpdate_SavesChanges()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new Prescription { PrescriptionId = id };
            var delta = new Delta<PrescriptionUpdateDto>();
            delta.TrySetPropertyValue(nameof(PrescriptionUpdateDto.Notes), "New notes");

            _mockRepo.Setup(repo => repo.GetAsync(
                    It.Is<Expression<Func<Prescription, bool>>>(expr => expr.Compile()(existing)),
                    It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()))
                .ReturnsAsync(existing);

           
            _mockRepo.Setup(r => r.GetEntityState(It.IsAny<Prescription>()))
                .Returns(EntityState.Modified);

            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdatePrescriptionAsync(id, delta);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.Complete(), Times.Once);
            _mockEventStore.Verify(e => e.SaveEvents(It.IsAny<IReadOnlyCollection<IPrescriptionEvent>>()), Times.Once);
        }

        [Fact]
        public async Task DeletePrescriptionAsync_InvalidId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeletePrescriptionAsync(Guid.Empty));
        }

        [Fact]
        public async Task DeletePrescriptionAsync_ExistingId_DeletesAndSaves()
        {
            // Arrange
            var id = Guid.NewGuid();
            var prescription = new Prescription { PrescriptionId = id };

            _mockRepo.Setup(repo => repo.GetAsync(
                    It.Is<Expression<Func<Prescription, bool>>>(expr => expr.Compile()(prescription)),
                    It.IsAny<Func<IQueryable<Prescription>, IIncludableQueryable<Prescription, object>>>()))
                .ReturnsAsync(prescription);
            
            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.DeletePrescriptionAsync(id);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.Remove(prescription), Times.Once);
            _mockRepo.Verify(r => r.Complete(), Times.Once);
            _mockEventStore.Verify(e => e.SaveEvents(It.IsAny<IReadOnlyCollection<IPrescriptionEvent>>()), Times.Once);
        }

        [Fact]
        public async Task CreatePrescriptionAsync_SaveFails_ThrowsException()
        {
            // Arrange
            var validDto = new PrescriptionCreateDto
            {
                DoctorId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                ConsultationId = Guid.NewGuid(),
                Items = { new PrescriptionItemCreateDto() }
            };
            var prescription = new Prescription()
            {
                FkDoctorId = validDto.DoctorId,
                FkPatientId = validDto.PatientId,
                FkConsultationId = validDto.ConsultationId,
            };
            _mockMapper.Setup(m => m.Map<Prescription>(validDto)).Returns(prescription);

            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreatePrescriptionAsync(validDto));
        }
    }
}
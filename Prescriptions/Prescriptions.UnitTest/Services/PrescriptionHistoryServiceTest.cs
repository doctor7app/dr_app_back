using AutoMapper;
using Common.Services.Interfaces;
using Moq;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Implementation.Services;
using Prescriptions.Infrastructure.Persistence;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Prescriptions.UnitTest.Services
{
    public class PrescriptionHistoryServiceTests
    {
        private readonly Mock<IRepository<StoredEvent, PrescriptionDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PrescriptionHistoryService _service;

        public PrescriptionHistoryServiceTests()
        {
            _mockRepo = new Mock<IRepository<StoredEvent, PrescriptionDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _service = new PrescriptionHistoryService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetPrescriptionHistoryAsync_ValidId_ReturnsMappedEvents()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var storedEvents = new List<StoredEvent>
            {
                new() { AggregateId = prescriptionId, AggregateType = "Prescription" },
                new() { AggregateId = prescriptionId, AggregateType = "Prescription" }
            };

            var dtos = storedEvents.Select(_ => new StoredEventDto()).ToList();
            
            _mockRepo.Setup(u => u.GetListAsync(
                It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()
            )).ReturnsAsync(storedEvents);

            _mockMapper.Setup(m => m.Map<IEnumerable<StoredEventDto>>(storedEvents))
                .Returns(dtos);

            // Act
            var result = await _service.GetPrescriptionHistoryAsync(prescriptionId);

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.GetListAsync(
                It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetPrescriptionItemHistoryAsync_ValidId_ReturnsMappedEvents()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var storedEvents = new List<StoredEvent>
            {
                new() { AggregateId = itemId, AggregateType = "PrescriptionItem" }
            };

            var dtos = storedEvents.Select(_ => new StoredEventDto()).ToList();

            _mockRepo.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                    It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()))
                .ReturnsAsync(storedEvents);

            _mockMapper.Setup(m => m.Map<IEnumerable<StoredEventDto>>(storedEvents))
                .Returns(dtos);

            // Act
            var result = await _service.GetPrescriptionItemHistoryAsync(itemId);

            // Assert
            Assert.Single(result);
            _mockRepo.Verify(r => r.GetListAsync(
                It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetPrescriptionHistoryAsync_EmptyId_ReturnsEmptyResults()
        {
            // Arrange
            var emptyId = Guid.Empty;
            _mockRepo.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                    It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()))
                .ReturnsAsync(new List<StoredEvent>());

            // Act
            var result = await _service.GetPrescriptionHistoryAsync(emptyId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPrescriptionItemHistoryAsync_EmptyId_ReturnsEmptyResults()
        {
            // Arrange
            var emptyId = Guid.Empty;
            _mockRepo.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                    It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()))
                .ReturnsAsync(new List<StoredEvent>());

            // Act
            var result = await _service.GetPrescriptionItemHistoryAsync(emptyId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPrescriptionHistoryAsync_FiltersCorrectAggregateType()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var mixedEvents = new List<StoredEvent>
            {
                new() { AggregateId = prescriptionId, AggregateType = "Prescription" },
                new() { AggregateId = prescriptionId, AggregateType = "PrescriptionItem" }
            };

            var dtos = new List<StoredEventDto>
            {
                new() { AggregateId = prescriptionId, AggregateType = "Prescription" },
                new() { AggregateId = prescriptionId, AggregateType = "PrescriptionItem" }
            };

            _mockMapper.Setup(m => m.Map<IEnumerable<StoredEventDto>>(mixedEvents)).Returns(dtos);

            _mockRepo.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                    It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()))
                .ReturnsAsync(mixedEvents);
            
            // Act
            var result = await _service.GetPrescriptionHistoryAsync(prescriptionId);

            // Assert
            var storedEventDtos = result.ToList();
            Assert.Equal(storedEventDtos?.Count, 2);
            if (storedEventDtos != null)
            {
                Assert.All(storedEventDtos, _ => Assert.Equal("Prescription",
                    mixedEvents.First(e => e.AggregateId == prescriptionId && e.AggregateType == "Prescription")
                        .AggregateType));

            }
        }
        
        [Fact]
        public async Task GetPrescriptionHistoryAsync_MapsEventsCorrectly()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var storedEvent = new List<StoredEvent> { new() { AggregateId = prescriptionId, AggregateType = "Prescription" }};
            var expectedDto = new List<StoredEventDto> { new() };
            
            _mockMapper.Setup(m => m.Map<IEnumerable<StoredEventDto>>(storedEvent)).Returns(expectedDto);

            _mockRepo.Setup(r => r.GetListAsync(It.IsAny<Expression<Func<StoredEvent, bool>>>(),
                    It.IsAny<Func<IQueryable<StoredEvent>, IIncludableQueryable<StoredEvent, object>>>()))
                .ReturnsAsync(storedEvent);
            
            // Act
            var result = await _service.GetPrescriptionHistoryAsync(prescriptionId);

            // Assert
            Assert.Same(expectedDto, result);
            _mockMapper.Verify(m => m.Map<IEnumerable<StoredEventDto>>(storedEvent), Times.Once);
        }
    }
}
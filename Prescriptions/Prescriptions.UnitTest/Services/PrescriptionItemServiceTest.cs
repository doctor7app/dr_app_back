using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Interfaces.Services;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Implementation.Services;
using Prescriptions.Infrastructure.Persistence;
using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using System.ComponentModel.DataAnnotations;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Domain.Interfaces;


namespace Prescriptions.UnitTest.Services
{
    public class PrescriptionItemServiceTests
    {
        private readonly Mock<IRepository<PrescriptionItem, PrescriptionDbContext>> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IEventStoreService> _mockEventStore;
        private readonly PrescriptionItemService _service;

        public PrescriptionItemServiceTests()
        {
            _mockRepo = new Mock<IRepository<PrescriptionItem, PrescriptionDbContext>>();
            _mockMapper = new Mock<IMapper>();
            _mockEventStore = new Mock<IEventStoreService>();
            _service = new PrescriptionItemService(_mockRepo.Object, _mockMapper.Object, _mockEventStore.Object);
        }

        [Fact]
        public async Task GetItemByIdAsync_InvalidIds_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GetItemByIdAsync(Guid.Empty, Guid.NewGuid()));
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GetItemByIdAsync(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public async Task GetItemByIdAsync_ValidIds_ReturnsMappedDto()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var item = new PrescriptionItem { PrescriptionItemId = itemId, FkPrescriptionId = prescriptionId };
            var dto = new PrescriptionItemDto { Id = itemId };

            _mockRepo.Setup(repo => repo.GetAsync(
                    It.Is<Expression<Func<PrescriptionItem, bool>>>(expr => expr.Compile()(item)),
                    It.IsAny<Func<IQueryable<PrescriptionItem>, IIncludableQueryable<PrescriptionItem, object>>>()))
                .ReturnsAsync(item);
            
            _mockMapper.Setup(m => m.Map<PrescriptionItemDto>(item)).Returns(dto);

            // Act
            var result = await _service.GetItemByIdAsync(prescriptionId, itemId);

            // Assert
            Assert.Equal(itemId, result.Id);
            _mockRepo.Verify(r => r.GetAsync(It.Is<Expression<Func<PrescriptionItem, bool>>>(expr => expr.Compile()(item)),
                It.IsAny<Func<IQueryable<PrescriptionItem>, IIncludableQueryable<PrescriptionItem, object>>>()), Times.Once);
        }

        [Fact]
        public async Task GetAllItemRelatedToPrescriptionByIdAsync_InvalidId_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.GetAllItemRelatedToPrescriptionByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetAllItemRelatedToPrescriptionByIdAsync_ValidId_ReturnsMappedDtos()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var items = new List<PrescriptionItem> { new PrescriptionItem() };
            var dtos = new List<PrescriptionItemDto> { new PrescriptionItemDto() };
            
            _mockRepo.Setup(u => u.GetListAsync(
                It.IsAny<Expression<Func<PrescriptionItem, bool>>>(),
                It.IsAny<Func<IQueryable<PrescriptionItem>, IIncludableQueryable<PrescriptionItem, object>>>()
            )).ReturnsAsync(items);

            _mockMapper.Setup(m => m.Map<IEnumerable<PrescriptionItemDto>>(items)).Returns(dtos);

            // Act
            var result = await _service.GetAllItemRelatedToPrescriptionByIdAsync(prescriptionId);

            // Assert
            Assert.Single(result);
            _mockRepo.Verify(r => r.GetListAsync(It.IsAny<Expression<Func<PrescriptionItem, bool>>>(),
                It.IsAny<Func<IQueryable<PrescriptionItem>, IIncludableQueryable<PrescriptionItem, object>>>()), Times.Once);
        }

        [Fact]
        public async Task CreatePrescriptionItem_InvalidId_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionItem(Guid.Empty, new PrescriptionItemCreateDto()));
        }

        [Fact]
        public async Task CreatePrescriptionItem_ValidRequest_SavesItemAndEvents()
        {
            // Arrange
            var prescriptionId = Guid.NewGuid();
            var dto = new PrescriptionItemCreateDto();
            var item = new PrescriptionItem();

            _mockMapper.Setup(m => m.Map<PrescriptionItem>(dto)).Returns(item);
            _mockRepo.Setup(r => r.AddAsync(item));
            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.CreatePrescriptionItem(prescriptionId, dto);

            // Assert
            Assert.True(result);
            Assert.Equal(prescriptionId, item.FkPrescriptionId);
            _mockRepo.Verify(r => r.AddAsync(item), Times.Once);
            _mockEventStore.Verify(e => e.SaveEvents(item.Events), Times.Once);
        }

        [Fact]
        public async Task UpdateItemAsync_InvalidIds_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateItemAsync(Guid.Empty, Guid.NewGuid(), new Delta<PrescriptionItemUpdateDto>()));
            await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateItemAsync(Guid.NewGuid(), Guid.Empty, new Delta<PrescriptionItemUpdateDto>()));
        }

        [Fact]
        public async Task UpdateItemAsync_NonExistingItem_ThrowsException()
        {
            // Arrange
            var invalidId = Guid.Empty;
            var patchDto = new Delta<PrescriptionItemUpdateDto>();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateItemAsync(invalidId, invalidId, patchDto));
        }

        [Fact]
        public async Task UpdateItemAsync_ValidUpdate_SavesChanges()
        {
            // Arrange
            var idItem = Guid.NewGuid();
            var idPrescription = Guid.NewGuid();
            var existing = new PrescriptionItem() { PrescriptionItemId = idItem,FkPrescriptionId = idPrescription};
            var delta = new Delta<PrescriptionItemUpdateDto>();
            delta.TrySetPropertyValue(nameof(PrescriptionItemUpdateDto.Dosage), "New noteDosages");

            _mockRepo.Setup(repo => repo.GetAsync(
                    It.Is<Expression<Func<PrescriptionItem, bool>>>(expr => expr.Compile()(existing)),
                    It.IsAny<Func<IQueryable<PrescriptionItem>, IIncludableQueryable<PrescriptionItem, object>>>()))
                .ReturnsAsync(existing);


            _mockRepo.Setup(r => r.GetEntityState(It.IsAny<PrescriptionItem>()))
                .Returns(EntityState.Modified);

            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateItemAsync(idPrescription, idItem, delta);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(r => r.Complete(), Times.Once);
            _mockEventStore.Verify(e => e.SaveEvents(It.IsAny<IReadOnlyCollection<IPrescriptionEvent>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteItemAsync_InvalidIds_ThrowsException()
        {
            await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteItemAsync(Guid.Empty, Guid.NewGuid()));
            await Assert.ThrowsAsync<Exception>(() =>
                _service.DeleteItemAsync(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public async Task DeleteItemAsync_ValidRequest_RemovesItemAndSaves()
        {
            // Arrange
            var idPrescription = Guid.NewGuid();
            var idItem = Guid.NewGuid();
            var diagnostic = new PrescriptionItem() { PrescriptionItemId = idItem, FkPrescriptionId = idPrescription };

            _mockRepo.Setup(u => u.GetAsync(x => x.PrescriptionItemId == idItem && x.FkPrescriptionId == idPrescription, null))
                .ReturnsAsync(diagnostic);
            _mockRepo.Setup(repo => repo.Remove(diagnostic));
            _mockRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteItemAsync(idPrescription, idItem);

            // Assert
            Assert.True((bool)result);
            _mockRepo.Verify(u => u.Remove(diagnostic), Times.Once);
            _mockRepo.Verify(u => u.Complete(), Times.Once);
        }

        [Fact]
        public async Task CreatePrescriptionItem_SaveFails_ThrowsException()
        {
            // Arrange
            var validDto = new PrescriptionItemCreateDto();
            var prescription = new PrescriptionItem();
            _mockMapper.Setup(m => m.Map<PrescriptionItem>(validDto)).Returns(prescription);

            _mockRepo.Setup(r => r.Complete()).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _service.CreatePrescriptionItem(Guid.NewGuid(), validDto));
        }
    }
}
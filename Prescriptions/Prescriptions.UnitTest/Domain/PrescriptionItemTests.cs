using Prescriptions.Domain.Events;
using Prescriptions.Domain.Models;

namespace Prescriptions.UnitTest.Domain
{
    public class PrescriptionItemTests
    {
        [Fact]
        public void Update_UpdatesAllProperties()
        {
            // Arrange
            var original = new PrescriptionItem
            {
                DrugName = "Original",
                Dosage = "10mg",
                IsPrn = false
            };

            var update = new PrescriptionItem
            {
                DrugName = "Updated",
                Dosage = "20mg",
                IsPrn = true,
                Notes = "New notes"
            };

            // Act
            original.Update(update);

            // Assert
            Assert.Equal("Updated", original.DrugName);
            Assert.Equal("20mg", original.Dosage);
            Assert.True(original.IsPrn);
            Assert.Equal("New notes", original.Notes);
        }

        [Fact]
        public void AddPrescriptionItemEvent_AddsCorrectEvent()
        {
            // Arrange
            var item = new PrescriptionItem
            {
                PrescriptionItemId = Guid.NewGuid(),
                DrugName = "Test Drug",
                Dosage = "5mg"
            };

            // Act
            item.AddPrescriptionItemEvent();

            // Assert
            var @event = item.Events.Single() as PrescriptionItemCreatedEvent;
            Assert.Equal(item.DrugName, @event?.DrugName);
            Assert.Equal(item.Dosage, @event?.Dosage);
        }

        [Fact]
        public void UpdatePrescriptionItemEvent_AddsUpdatedEvent()
        {
            // Arrange
            var item = new PrescriptionItem
            {
                PrescriptionItemId = Guid.NewGuid(),
                DrugName = "Updated Drug",
                Dosage = "15mg"
            };

            // Act
            item.UpdatePrescriptionItemEvent();

            // Assert
            var @event = item.Events.Single() as PrescriptionItemUpdatedEvent;
            Assert.Equal(item.PrescriptionItemId, @event?.PrescriptionItemId);
        }

        [Fact]
        public void RemovePrescriptionItemEvent_AddsCorrectEvent()
        {
            // Arrange
            var item = new PrescriptionItem { PrescriptionItemId = Guid.NewGuid() };

            // Act
            item.RemovePrescriptionItemEvent();

            // Assert
            var @event = item.Events.Single() as PrescriptionItemDeletedEvent;
            Assert.Equal(item.PrescriptionItemId, @event?.PrescriptionItemId);
        }

        [Fact]
        public void Update_WithNullValues_UpdatesCorrectly()
        {
            // Arrange
            var original = new PrescriptionItem
            {
                Notes = "Original notes",
                Duration = "7 days"
            };

            var update = new PrescriptionItem
            {
                Notes = null,
                Duration = null
            };

            // Act
            original.Update(update);

            // Assert
            Assert.Null(original.Notes);
            Assert.Null(original.Duration);
        }
    }
}
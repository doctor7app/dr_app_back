using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Application.Dtos.Prescriptions;

namespace Prescriptions.Api.Helpers;

public static class EdmModelBuilder
{
    public static IEdmModel Build()
    {
        var builder = new ODataConventionModelBuilder
        {
            Namespace = "Prescription.API",
            ContainerName = "DefaultContainer"
        };

        // 1. PrescriptionDto (Entity)
        var prescription = builder.EntityType<PrescriptionDto>();
        prescription.HasKey(p => p.Id);
        prescription.Property(p => p.IssuedAt);
        prescription.Property(p => p.Notes);
        prescription.EnumProperty(p => p.Status);
        prescription.EnumProperty(p => p.ConsultationType);
        prescription.Property(p => p.ExpirationDate);
        prescription.Property(p => p.PatientId);
        prescription.Property(p => p.ConsultationId);
        prescription.Property(p => p.DoctorId);
        prescription.HasMany(p => p.Items).Contained();

        // 2. PrescriptionItemDto (Entity)
        var item = builder.EntityType<PrescriptionItemDto>();
        item.HasKey(i => i.Id);
        item.Property(i => i.DrugName);
        item.Property(i => i.Dosage);
        item.Property(i => i.Frequency);
        item.Property(i => i.Duration);
        item.Property(i => i.Instructions);
        item.EnumProperty(i => i.MedicationType);
        item.Property(i => i.IsEssential);
        item.EnumProperty(i => i.Route);
        item.Property(i => i.TimeOfDay);
        item.Property(i => i.MealInstructions);
        item.Property(i => i.IsPrn);
        item.Property(i => i.Notes);
        
        // 6. EntitySets
        builder.EntitySet<PrescriptionDto>("Prescriptions");
        builder.EntitySet<PrescriptionItemDto>("Items");
        builder.EntitySet<StoredEventDto>("History");

        var history = builder.EntityType<StoredEventDto>();

        // Function for Prescription History
        history.Collection
            .Function("GetPrescriptionHistory")
            .ReturnsCollectionFromEntitySet<StoredEventDto>("History")
            .Parameter<Guid>("key");

        // Function for Item History
        history.Collection
            .Function("GetItemHistory")
            .ReturnsCollectionFromEntitySet<StoredEventDto>("History")
            .Parameter<Guid>("key");

        // 7. Patch Entities
        var updateDto = builder.ComplexType<PrescriptionUpdateDto>();
        updateDto.Property(u => u.Notes);
        updateDto.EnumProperty(u => u.Status);
        updateDto.Property(u => u.ExpirationDate);


        var updateItemDto = builder.ComplexType<PrescriptionItemUpdateDto>();
        updateItemDto.Property(u => u.Dosage);
        updateItemDto.Property(u => u.Frequency);
        updateItemDto.Property(u => u.Duration);
        updateItemDto.Property(u => u.Instructions);
        updateItemDto.Property(u => u.IsPrn);

        return builder.GetEdmModel();
    }
}
using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Dme.Api.Helpers
{
    public static class EdmModelBuilder
    {
        public static IEdmModel Build()
        {
            var builder = new ODataConventionModelBuilder
            {
                Namespace = "Dme.API", ContainerName = "DefaultContainer"
            };

            builder.EntitySet<ConsultationsReadDto>("Consultations").EntityType.HasKey(a => a.Id);

            var patients = builder.EntitySet<DmeReadDto>("Dmes").EntityType;
            
            patients.HasKey(a => a.Id);
            patients.HasMany(a => a.Consultations);
            
            builder.EntitySet<ConsultationsReadDto>("Treatments").EntityType.HasKey(a => a.Id);
            builder.EntitySet<ConsultationsReadDto>("Diagnostics").EntityType.HasKey(a => a.Id);

            var consultations = builder.EntitySet<ConsultationsReadDto>("Consultations").EntityType;
            consultations.HasKey(a => a.Id);
            consultations.HasMany(a => a.Treatments);
            consultations.HasMany(a => a.Diagnostics);
            
            //builder.EnableLowerCamelCase();
            return builder.GetEdmModel();
        }
    }
}
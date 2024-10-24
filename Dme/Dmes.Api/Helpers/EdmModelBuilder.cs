using Dme.Dtos.Consultations;
using Dme.Dtos.Dmes;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Dmes.Api.Helpers
{
    public static class EdmModelBuilder
    {
        public static IEdmModel Build()
        {
            var builder = new ODataConventionModelBuilder
            {
                Namespace = "Dme.API", ContainerName = "DefaultContainer"
            };
            
            var patients = builder.EntitySet<DmeReadDto>("Dmes").EntityType;
            
            patients.HasKey(a => a.Id);
            patients.HasMany(a => a.Consultations);
            
            var consultations = builder.EntitySet<ConsultationsReadDto>("Consultations").EntityType;
            consultations.HasKey(a => a.Id);
            consultations.HasMany(a => a.Treatments);
            consultations.HasMany(a => a.Diagnostics);
            
            //builder.EnableLowerCamelCase();
            return builder.GetEdmModel();
        }
    }
}
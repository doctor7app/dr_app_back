using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Diagnostics;
using Dme.Application.DTOs.Dmes;
using Dme.Application.DTOs.Treatments;
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
                Namespace = "Dme.API",
                ContainerName = "DefaultContainer"
            };

            var dme = builder.EntitySet<DmeReadDto>("Dmes").EntityType;
            dme.HasKey(a => a.Id);
            dme.HasMany(a => a.Consultations);

            builder.EntitySet<TreatmentsReadDto>("Treatments").EntityType.HasKey(a => a.Id);
            builder.EntitySet<DiagnosticsReadDto>("Diagnostics").EntityType.HasKey(a => a.Id);

            var consultations = builder.EntitySet<ConsultationsReadDto>("Consultations").EntityType;
            consultations.HasKey(a => a.Id);
            consultations.HasMany(a => a.Treatments);
            consultations.HasMany(a => a.Diagnostics);

            //builder.EnableLowerCamelCase();
            return builder.GetEdmModel();
        }

    }
}
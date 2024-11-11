using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Patients.Application.DTOs.Patient;

namespace Patients.Api.Helpers
{
    public static class EdmModelBuilder
    {
        public static IEdmModel Build()
        {
            var builder = new ODataConventionModelBuilder
            {
                Namespace = "Patient.API", ContainerName = "DefaultContainer"
            };
            
            var patients = builder.EntitySet<PatientDto>("Patients").EntityType;
            
            patients.HasKey(a => a.Id);
            patients.HasMany(a => a.Adresses);
            patients.HasMany(a => a.Contacts);
            patients.HasMany(a => a.MedicalInfos);
            
            //builder.EnableLowerCamelCase();
            return builder.GetEdmModel();
        }
    }
}
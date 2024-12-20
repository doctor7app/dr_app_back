﻿using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Patients.Application.DTOs.Adresse;
using Patients.Application.DTOs.Contact;
using Patients.Application.DTOs.MedicalInfo;
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

            builder.EntitySet<AdresseDto>("Adresses").EntityType.HasKey(a => a.Id);
            builder.EntitySet<ContactDto>("Contacts").EntityType.HasKey(a => a.Id);
            builder.EntitySet<MedicalInfoDto>("MedicalInfos").EntityType.HasKey(a => a.Id);

            var patients = builder.EntitySet<PatientDto>("Patients").EntityType;
            
            patients.HasKey(a => a.Id);
            
            patients.HasMany(a => a.Adresses);
            patients.HasMany(a => a.Contacts);
            patients.HasMany(a => a.MedicalInfos);
            
            return builder.GetEdmModel();
        }
    }
}
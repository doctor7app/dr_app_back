using Common.Services.Interfaces;
using Patients.Application.DTOs.Patient;
using Patients.Domain.Models;

namespace Patients.Application.Interfaces;

public interface IPatientService : IServiceGeneric<Patient, PatientDto,PatientCreateDto,PatientUpdateDto>
{
    
}
using Common.Services.Interfaces;
using Patients.Application.DTOs.Patient;

namespace Patients.Application.Interfaces;

public interface IPatientService : IServiceGeneric<PatientDto,PatientCreateDto,PatientUpdateDto>
{
    
}
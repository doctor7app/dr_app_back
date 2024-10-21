using Common.Services.Interfaces;
using Patients.Dtos.Patient;

namespace Patients.Services.Interfaces;

public interface IPatientService : IServiceGeneric<PatientDto,PatientCreateDto,PatientUpdateDto>
{
    
}
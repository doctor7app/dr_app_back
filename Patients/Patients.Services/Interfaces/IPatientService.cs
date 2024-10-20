using Common.Services.Interfaces;
using Patients.Dtos.Classes.Patient;

namespace Patients.Services.Interfaces;

public interface IPatientService : IServiceGeneric<PatientDto,PatientCreateDto,PatientUpdateDto>
{
    
}
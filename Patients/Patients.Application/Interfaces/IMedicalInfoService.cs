using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.MedicalInfo;

namespace Patients.Application.Interfaces;

public interface IMedicalInfoService : IServiceGeneric<MedicalInfoDto, MedicalInfoCreateDto, MedicalInfoDto>
{
    Task<object> GetMedicalInfo(Guid patientId, Guid medicalInfoId);
    Task<IEnumerable<MedicalInfoDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, MedicalInfoCreateDto entity);
    Task<object> Update(Guid patientId, Guid medicalInfoId, Delta<MedicalInfoDto> entity);
    Task<object> Delete(Guid patientId, Guid medicalInfoId);
}
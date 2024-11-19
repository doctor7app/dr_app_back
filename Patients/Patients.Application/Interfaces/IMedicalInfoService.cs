using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.MedicalInfo;

namespace Patients.Application.Interfaces;

public interface IMedicalInfoService
{
    Task<object> GetMedicalInfo(Guid patientId, Guid medicalInfoId);
    Task<IEnumerable<MedicalInfoDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, MedicalInfoCreateDto entity);
    Task<object> Patch(Guid patientId, Guid medicalInfoId, MedicalInfoPatchDto entity);
    Task<object> Delete(Guid patientId, Guid medicalInfoId);
}
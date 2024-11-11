using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.MedicalInfo;
using Patients.Domain.Models;

namespace Patients.Application.Interfaces;

public interface IMedicalInfoService : IServiceGeneric<MedicalInformation, MedicalInfoDto, MedicalInfoCreateDto, MedicalInfoDto>
{
    Task<object> GetMedicalInfo(Guid patientId, Guid medicalInfoId);
    Task<IEnumerable<MedicalInfoDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, MedicalInfoCreateDto entity);
    Task<object> Update(Guid patientId, Guid medicalInfoId, Delta<MedicalInfoDto> entity);
    Task<object> Delete(Guid patientId, Guid medicalInfoId);
}
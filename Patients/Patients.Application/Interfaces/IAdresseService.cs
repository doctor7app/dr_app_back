using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.Adresse;
using Patients.Domain.Models;

namespace Patients.Application.Interfaces;

public interface IAdresseService : IServiceGeneric<Adresse,AdresseDto, AdresseCreateDto, AdresseDto>
{
    Task<object> GetAdresse(Guid patientId,Guid adresseId);
    Task<IEnumerable<AdresseDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, AdresseCreateDto entity);
    Task<object> Update(Guid patientId, Guid adresseId, Delta<AdresseDto> entity);
    Task<object> Delete(Guid patientId, Guid adresseId);
}
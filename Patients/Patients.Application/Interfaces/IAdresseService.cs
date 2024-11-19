using Patients.Application.DTOs.Adresse;

namespace Patients.Application.Interfaces;

public interface IAdresseService
{
    Task<object> GetAdresse(Guid patientId,Guid adresseId);
    Task<IEnumerable<AdresseDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, AdresseCreateDto entity);
    Task<object> Patch(Guid patientId, Guid adresseId, AdressePatchDto entity);
    Task<object> Delete(Guid patientId, Guid adresseId);
}
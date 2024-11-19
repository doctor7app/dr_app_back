using Patients.Application.DTOs.Contact;

namespace Patients.Application.Interfaces;

public interface IContactService
{
    Task<object> GetContact(Guid patientId, Guid contactId);
    Task<IEnumerable<ContactDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, ContactCreateDto entity);
    Task<object> Patch(Guid patientId, Guid contactId, ContactPatchDto entity);
    Task<object> Delete(Guid patientId, Guid contactId);
}
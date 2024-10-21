using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Dtos.Contact;

namespace Patients.Services.Interfaces;

public interface IContactService : IServiceGeneric<ContactDto, ContactCreateDto, ContactDto>
{
    Task<object> GetContact(Guid patientId, Guid contactId);
    Task<IEnumerable<ContactDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, ContactCreateDto entity);
    Task<object> Update(Guid patientId, Guid contactId, Delta<ContactDto> entity);
    Task<object> Delete(Guid patientId, Guid contactId);
}
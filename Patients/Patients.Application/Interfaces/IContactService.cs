using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.Contact;
using Patients.Domain.Models;

namespace Patients.Application.Interfaces;

public interface IContactService : IServiceGeneric<Contact,ContactDto, ContactCreateDto, ContactDto>
{
    Task<object> GetContact(Guid patientId, Guid contactId);
    Task<IEnumerable<ContactDto>> GetRelative(Guid patientId);
    Task<object> Create(Guid patientId, ContactCreateDto entity);
    Task<object> Update(Guid patientId, Guid contactId, Delta<ContactDto> entity);
    Task<object> Delete(Guid patientId, Guid contactId);
}
using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.Contact;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Implementation;

public class ContactService : IContactService
{
    private readonly IRepository<Contact, PatientDbContext> _work;
    private readonly IMapper _mapper;
    public ContactService(IRepository<Contact, PatientDbContext> work, IMapper mapper)
    {
        _work = work;
        _mapper = mapper;
    }


    public async Task<object> GetContact(Guid patientId, Guid contactId)
    {
        if (patientId == Guid.Empty || contactId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.ContactId == contactId);
        return _mapper.Map<ContactDto>(obj);
    }

    public async Task<IEnumerable<ContactDto>> GetRelative(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetListAsync(x => x.FkIdPatient == patientId);
        return _mapper.Map<IEnumerable<ContactDto>>(obj);
    }

    public async Task<object> Create(Guid patientId, ContactCreateDto entity)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        entity.IdPatient = patientId;
        var item = _mapper.Map<Contact>(entity);
        await _work.AddAsync(item);
        return await _work.Complete();
    }

    public async Task<object> Update(Guid patientId, Guid contactId, Delta<ContactDto> entity)
    {
        if (patientId == Guid.Empty || contactId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var entityToUpdate = await _work.GetAsync(z => z.ContactId == contactId && z.FkIdPatient == patientId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {contactId} n'existe pas dans la base de données!");
        }
        var entityDto = _mapper.Map<ContactDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _work.Complete();
    }

    public async Task<object> Delete(Guid patientId, Guid contactId)
    {
        if (patientId == Guid.Empty || contactId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.ContactId == contactId);
        _work.Remove(obj);
        return await _work.Complete();
    }
}
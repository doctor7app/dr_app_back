using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Contracts.Messages.Patients;
using MassTransit;
using Patients.Application.DTOs.Contact;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Implementation;

public class ContactService : IContactService
{
    private readonly IRepository<Contact, PatientDbContext> _work;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    public ContactService(IRepository<Contact, PatientDbContext> work, 
        IMapper mapper, 
        IPublishEndpoint publishEndpoint)
    {
        _work = work;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
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
        var item = _mapper.Map<Contact>(entity);
        item.FkIdPatient = patientId;
        await _work.AddAsync(item);
        var result =  await _work.Complete() >0;
        if (!result)
        {
            throw new Exception("Could not save Contact to database");
        }
        var newPatient = _mapper.Map<ContactDto>(item);
        await _publishEndpoint.Publish(_mapper.Map<ContactCreatedEvent>(newPatient));
        return true;
    }

    public async Task<object> Patch(Guid patientId, Guid contactId, ContactPatchDto entity)
    {
        if (patientId == Guid.Empty || contactId == Guid.Empty)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entityToUpdate = await _work.GetAsync(z => z.ContactId == contactId && z.FkIdPatient == patientId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {contactId} n'existe pas dans la base de données!");
        }
        entityToUpdate.UpdateWithDto(entity);
        var result =  await _work.Complete() >0;
        if (!result)
        {
            throw new Exception("Could not update Contact to database");
        }
        var updatedPatient = _mapper.Map<ContactDto>(entityToUpdate);
        var entityToPublish = _mapper.Map<ContactUpdatedEvent>(updatedPatient);
        entityToPublish.Id = entityToUpdate.ContactId;
        await _publishEndpoint.Publish(entityToPublish);
        return true;
    }

    public async Task<object> Delete(Guid patientId, Guid contactId)
    {
        if (patientId == Guid.Empty || contactId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.ContactId == contactId);
        if (obj == null)
        {
            throw new Exception($"L'élement avec l'id {contactId} n'existe pas dans la base de données!");
        }
        _work.Remove(obj);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Contact from database");
        }
        var deletedPatient = new ContactDeletedEvent { Id = contactId };
        await _publishEndpoint.Publish(deletedPatient);
        return true;
    }
}
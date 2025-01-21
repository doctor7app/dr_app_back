using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Contracts.Messages.Patients;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Implementation;

public class PatientService : IPatientService
{
    private readonly IRepository<Patient, PatientDbContext> _work;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public PatientService(IRepository<Patient,
            PatientDbContext> work,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
    {
        _work = work;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<object> Get(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PatientId == id, includes:
                        a => a.Include(z => z.Adresses)
                            .Include(z => z.Contacts)
                            .Include(z => z.MedicalInformations));

        return _mapper.Map<PatientDto>(obj);
    }

    public async Task<IEnumerable<PatientDto>> Get()
    {
        var obj = await _work.GetListAsync(includes:
            a => a.Include(z => z.Adresses)
                .Include(z => z.Contacts)
                .Include(z => z.MedicalInformations));
        return _mapper.Map<IEnumerable<PatientDto>>(obj);
    }

    public async Task<object> Create(PatientCreateDto entity)
    {
        var item = _mapper.Map<Patient>(entity);
        await _work.AddAsync(item);
        var newPatient = _mapper.Map<PatientDto>(item);
        await _publishEndpoint.Publish(_mapper.Map<PatientCreatedEvent>(newPatient));
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save Patient to database");
        }
        return true;
    }

    public async Task<object> Patch(Guid key, PatientPatchDto entity)
    {
        if (key == Guid.Empty)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entityToUpdate = await _work.GetAsync(z => z.PatientId == key);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {key} n'existe pas dans la base de données!");
        }
        entityToUpdate.UpdateWithDto(entity);

        var updatedPatient = _mapper.Map<PatientDto>(entityToUpdate);
        var entityToPublish = _mapper.Map<PatientUpdatedEvent>(updatedPatient);
        entityToPublish.Id = key;
        await _publishEndpoint.Publish(entityToPublish);

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not update Patient to database");
        }

        return true;
    }

    public async Task<object> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.PatientId == id);
        _work.Remove(obj);

        var deletedPatient = new PatientDeletedEvent { Id = id };
        await _publishEndpoint.Publish(deletedPatient);

        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not Delete Patient from database");
        }


        return true;
    }
}
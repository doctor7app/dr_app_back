using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Implementation;

public class PatientService : IPatientService
{
    private readonly IRepository<Patient, PatientDbContext> _work;
    private readonly IMapper _mapper;
    public PatientService(IRepository<Patient, PatientDbContext> work, IMapper mapper)
    {
        _work = work;
        _mapper = mapper;
    }

    public async Task<object> Get(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.PatientId == id);
        return _mapper.Map<PatientDto>(obj);
    }

    public async Task<IEnumerable<PatientDto>> Get()
    {
        var obj = await _work.GetListAsync();
        return _mapper.Map<IEnumerable<PatientDto>>(obj);
    }

    public async Task<object> Create(PatientCreateDto entity)
    {
        var item = _mapper.Map<Patient>(entity);
        await _work.AddAsync(item);
        return await _work.Complete();
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
        return await _work.Complete();
    }

    public async Task<object> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.PatientId == id);
        _work.Remove(obj);
        return await _work.Complete();
    }
}
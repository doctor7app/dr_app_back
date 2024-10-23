using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Domain.Models;
using Dme.Dtos.Treatments;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Implementation;

public class TreatmentService : ITreatmentService
{
    private readonly IRepository<Treatments> _repository;
    private readonly IMapper _mapper;

    public TreatmentService(IRepository<Treatments> repository,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<object> Get(Guid id,Guid idConsultation)
    {
        if (id.IsNullOrEmpty() || idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetAsync(x => x.TreatmentsId == id && x.FkIdConsultation == idConsultation);
        return _mapper.Map<TreatmentsReadDto>(obj);
    }

    public async Task<IEnumerable<TreatmentsReadDto>> Get(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetListAsync(a => a.FkIdConsultation == idConsultation);
        return _mapper.Map<IEnumerable<TreatmentsReadDto>>(obj);
    }

    public async Task<object> Create(TreatmentsCreateDto entity)
    {
        if (entity == null || entity.ConsultationId.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var itemToCreate = _mapper.Map<Treatments>(entity);

        await _repository.AddAsync(itemToCreate);
        await _repository.Complete();
        return true;
    }

    public async Task<object> Update(Guid key, Delta<TreatmentsUpdateDto> entity)
    {
        if (key.IsNullOrEmpty() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repository.GetAsync(x => x.TreatmentsId == key);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        var entityDto = _mapper.Map<TreatmentsUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repository.Complete();
    }

    public async Task<object> Delete(Guid id)
    {
        if (id.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repository.GetAsync(x => x.TreatmentsId == id);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        return await _repository.Complete();
    }
}
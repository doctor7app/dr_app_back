using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Domain.Models;
using Dme.Dtos.Consultations;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace Dme.Services.Implementation;

public class ConsultationService : IConsultationService
{
    private readonly IRepository<Consultations> _repository;
    private readonly IMapper _mapper;

    public ConsultationService(IRepository<Consultations> repository,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<object> Get(Guid id,Guid idDme)
    {
        if (id.IsNullOrEmpty() || idDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetAsync(x => x.ConsultationId == id  && x.FkIdDme == idDme, includes: z => z.Include(a => a.Diagnostics).Include(a=>a.Treatments));
        return _mapper.Map<ConsultationsReadDto>(obj);
    }

    public async Task<IEnumerable<ConsultationsReadDto>> Get(Guid idDme)
    {
        if (idDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetListAsync(x => x.ConsultationId == idDme, includes: z => z.Include(a => a.Diagnostics).Include(a => a.Treatments));
        return _mapper.Map<IEnumerable<ConsultationsReadDto>>(obj);
    }

    public async Task<object> Create(ConsultationsCreateDto entity)
    {
        if (entity == null || entity.IdDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var itemToCreate = _mapper.Map<Domain.Models.Consultations>(entity);

        await _repository.AddAsync(itemToCreate);
        await _repository.Complete();
        return true;
    }

    public async Task<object> Update(Guid idConsultation, Delta<ConsultationsUpdateDto> entity)
    {
        if (idConsultation.IsNullOrEmpty() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repository.GetAsync(x => x.ConsultationId == idConsultation);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        var entityDto = _mapper.Map<ConsultationsUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repository.Complete();
    }

    public async Task<object> Delete(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repository.GetAsync(x => x.ConsultationId == idConsultation);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        return await _repository.Complete();
    }
}
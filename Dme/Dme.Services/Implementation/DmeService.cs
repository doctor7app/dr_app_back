using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Dtos.Dmes;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace Dme.Services.Implementation;

public class DmeService : IDmeService
{
    private readonly IRepository<Domain.Models.Dme> _repository;
    private readonly IMapper _mapper;

    public DmeService(IRepository<Domain.Models.Dme> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<object> Get(Guid id,Guid doctorId)
    {
        if (id.IsNullOrEmpty() || doctorId.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetAsync(x => x.DmeId == id && x.FkIdDoctor == doctorId,includes:z=>z.Include(a => a.Consultations));
        return _mapper.Map<DmeReadDto>(obj);
    }

    public async Task<IEnumerable<DmeReadDto>> Get(Guid doctorId)
    {
        if (doctorId.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var items = await _repository.GetListAsync(x => x.FkIdDoctor == doctorId, includes: z => z.Include(a => a.Consultations));
        return _mapper.Map<IEnumerable<DmeReadDto>>(items);
    }

    public async Task<object> Create(DmeCreateDto entity)
    {
        if (entity== null || entity.DoctorId.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var itemToCreate = _mapper.Map<Domain.Models.Dme>(entity);

        await _repository.AddAsync(itemToCreate);
        await _repository.Complete();
        return true;
    }

    public async Task<object> Update(Guid idDme, Delta<DmeUpdateDto> entity)
    {
        if (idDme.IsNullOrEmpty() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repository.GetAsync(x => x.DmeId == idDme);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        var entityDto = _mapper.Map<DmeUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repository.Complete();

    }

    public async Task<object> Delete(Guid idDme)
    {
        if (idDme.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repository.GetAsync(x => x.DmeId == idDme);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        return await _repository.Complete();
    }
}
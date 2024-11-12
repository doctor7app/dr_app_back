using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;
using Dme.Infrastructure.Persistence;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace Dme.Infrastructure.Implementation;

public class DmeService : IDmeService
{
    private readonly IRepository<Domain.Models.Dme, DmeDbContext> _repository;
    private readonly IMapper _mapper;

    public DmeService(IRepository<Domain.Models.Dme, DmeDbContext> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<object> Get(Guid id)
    {
        if (id.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetAsync(x => x.DmeId == id,includes:z=>z.Include(a => a.Consultations));
        return _mapper.Map<DmeReadDto>(obj);
    }

    public async Task<IEnumerable<DmeReadDto>> Get()
    {
        var items = await _repository.GetListAsync(includes: z => z.Include(a => a.Consultations));
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
        return await _repository.Complete();
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
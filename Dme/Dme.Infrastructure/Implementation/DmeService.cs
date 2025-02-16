using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Contracts.Messages.Dmes;
using Dme.Application.DTOs.Dmes;
using Dme.Application.Interfaces;
using Dme.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Dme.Infrastructure.Implementation;

public class DmeService : IDmeService
{
    private readonly IRepository<Domain.Models.Dme, DmeDbContext> _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public DmeService(IRepository<Domain.Models.Dme, 
            DmeDbContext> repository, 
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<object> Get(Guid id)
    {
        if (id.IsNullOrEmptyGuid())
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
        if (entity== null || entity.DoctorId.IsNullOrEmptyGuid())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var itemToCreate = _mapper.Map<Domain.Models.Dme>(entity);
        await _repository.AddAsync(itemToCreate);
        
        var result =  await _repository.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save data to the database");
        }
        var newDme = _mapper.Map<DmeReadDto>(itemToCreate);
        await _publishEndpoint.Publish(_mapper.Map<DmeCreatedEvent>(newDme));
        return true;
    }

    public async Task<object> Patch(Guid idDme, DmePatchDto entity)
    {
        if (idDme.IsNullOrEmptyGuid() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repository.GetAsync(x => x.DmeId == idDme);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        entityToUpdate.UpdateWithDto(entity);
        
        var result =  await _repository.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save data to the Database");
        }
        var updatedDme = _mapper.Map<DmePatchDto>(entityToUpdate);
        var entityToPublish = _mapper.Map<DmeUpdatedEvent>(updatedDme);
        entityToPublish.Id = idDme;
        await _publishEndpoint.Publish(entityToPublish);
        return true;

    }

    public async Task<object> Delete(Guid idDme)
    {
        if (idDme.IsNullOrEmptyGuid())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repository.GetAsync(x => x.DmeId == idDme);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        
        var result =  await _repository.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save data to the Database");
        }
        await _publishEndpoint.Publish(new DmeDeletedEvent { Id = idDme });
        return true;
    }
}
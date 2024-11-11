﻿using AutoMapper;
using Common.Services.Implementation;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.Adresse;
using Patients.Application.Interfaces;
using Patients.Domain.Models;

namespace Patients.Infrastructure.Implementation;

public class AdresseService : ServiceGeneric<Adresse, AdresseDto, AdresseCreateDto, AdresseDto>, IAdresseService
{
    private readonly IRepository<Adresse> _work;
    private readonly IMapper _mapper;
    public AdresseService(IRepository<Adresse> work, IMapper mapper) : base(work, mapper)
    {
        _work = work;
        _mapper = mapper;
    }


    public async Task<object> GetAdresse(Guid patientId, Guid adresseId)
    {
        if (patientId == Guid.Empty || adresseId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.AdresseId == adresseId);
        return _mapper.Map<AdresseDto>(obj);
    }

    public async Task<IEnumerable<AdresseDto>> GetRelative(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetListAsync(x => x.FkIdPatient == patientId);
        return _mapper.Map<IEnumerable<AdresseDto>>(obj);
    }

    public async Task<object> Create(Guid patientId, AdresseCreateDto entity)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        
        entity.IdPatient = patientId;
        var item = _mapper.Map<Adresse>(entity);
        await _work.AddAsync(item);
        return await _work.Complete();
    }

    public async Task<object> Update(Guid patientId, Guid adresseId, Delta<AdresseDto> entity)
    {
        if (patientId == Guid.Empty || adresseId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var entityToUpdate = await _work.GetAsync(z => z.AdresseId == adresseId && z.FkIdPatient == patientId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {adresseId} n'existe pas dans la base de données!");
        }
        var entityDto = _mapper.Map<AdresseDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _work.Complete();
    }

    public async Task<object> Delete(Guid patientId, Guid adresseId)
    {
        if (patientId == Guid.Empty || adresseId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.AdresseId == adresseId);
        _work.Remove(obj);
        return await _work.Complete();
    }
}
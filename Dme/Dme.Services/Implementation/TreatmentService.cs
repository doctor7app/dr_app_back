﻿using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Domain.Models;
using Dme.Dtos.Treatments;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Implementation;

public class TreatmentService : ITreatmentService
{
    private readonly IRepository<Treatments> _repositoryTreatment;
    private readonly IRepository<Consultations> _repositoryConsultation;
    private readonly IMapper _mapper;

    public TreatmentService(IRepository<Treatments> repositoryTreatment,IRepository<Consultations> repositoryConsultation,IMapper mapper)
    {
        _repositoryTreatment = repositoryTreatment;
        _repositoryConsultation = repositoryConsultation;
        _mapper = mapper;
    }

    public async Task<object> GetTreatmentForConsultationById(Guid idConsultation, Guid idTreatment)
    {
        if (idTreatment.IsNullOrEmpty() || idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repositoryTreatment.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation);
        return _mapper.Map<TreatmentsReadDto>(obj);
    }

    public async Task<IEnumerable<TreatmentsReadDto>> GetAllTreatmentForConsultationById(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repositoryTreatment.GetListAsync(a => a.FkIdConsultation == idConsultation);
        return _mapper.Map<IEnumerable<TreatmentsReadDto>>(obj);
    }

    public async Task<object> CreateTreatmentForConsultation(Guid idConsultation,TreatmentsCreateDto entity)
    {
        if (entity == null || idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var tmpConsultation = await _repositoryConsultation.GetAsync(a => a.ConsultationId == idConsultation);
        if (tmpConsultation == null)
        {
            throw new Exception("Merci de vérifier les données saisie");
        }
        var itemToCreate = _mapper.Map<Treatments>(entity);
        itemToCreate.FkIdConsultation = idConsultation;
        await _repositoryTreatment.AddAsync(itemToCreate);
        return await _repositoryTreatment.Complete();
    }

    public async Task<object> UpdateTreatmentForConsultation(Guid idConsultation, Guid idTreatment, Delta<TreatmentsUpdateDto> entity)
    {
        if (idConsultation.IsNullOrEmpty() || idTreatment.IsNullOrEmpty() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repositoryTreatment.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        
        var entityDto = _mapper.Map<TreatmentsUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repositoryTreatment.Complete();
    }
    
    public async Task<object> DeleteTreatmentForConsultation(Guid idConsultation, Guid idTreatment)
    {
        if (idConsultation.IsNullOrEmpty() || idTreatment.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repositoryTreatment.GetAsync(x => x.TreatmentsId == idTreatment && x.FkIdConsultation == idConsultation);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repositoryTreatment.Remove(entity);
        return await _repositoryTreatment.Complete();
    }
}
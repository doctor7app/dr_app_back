using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Application.DTOs.Consultations;
using Dme.Application.Interfaces;
using Dme.Domain.Models;
using Dme.Infrastructure.Persistence;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.EntityFrameworkCore;

namespace Dme.Infrastructure.Implementation;

public class ConsultationService : IConsultationService
{
    private readonly IRepository<Consultations,DmeDbContext> _repository;
    private readonly IMapper _mapper;

    public ConsultationService(IRepository<Consultations, DmeDbContext> repository,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    #region Consutlation Dme Implementation

    public async Task<object> GetConsultationForDme(Guid idDme, Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty() || idDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme, includes: z => z.Include(a => a.Diagnostics).Include(a => a.Treatments));
        return _mapper.Map<ConsultationsReadDto>(obj);
    }

    public async Task<IEnumerable<ConsultationsReadDto>> GetAllConsultationForDme(Guid idDme)
    {
        if (idDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _repository.GetListAsync(x => x.ConsultationId == idDme, includes: z => z.Include(a => a.Diagnostics).Include(a => a.Treatments));
        return _mapper.Map<IEnumerable<ConsultationsReadDto>>(obj);
    }

    public async Task<object> CreateConsultationForDme(Guid idDme, ConsultationsCreateDto entity)
    {
        if (entity == null || idDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var itemToCreate = _mapper.Map<Consultations>(entity);

        await _repository.AddAsync(itemToCreate);
        return await _repository.Complete();
    }

    public async Task<object> UpdateConsultationForDme(Guid idDme, Guid idConsultation, Delta<ConsultationsUpdateDto> entity)
    {
        if (idConsultation.IsNullOrEmpty() || entity == null || idDme.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repository.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        var entityDto = _mapper.Map<ConsultationsUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repository.Complete();
    }

    public async Task<object> DeleteConsultationForDme(Guid idDme, Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty() || idDme.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }

        var entity = await _repository.GetAsync(x => x.ConsultationId == idConsultation && x.FkIdDme == idDme);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        return await _repository.Complete();
    }


    #endregion
    
    #region Consultation

    public async Task<object> GetConsultationById(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être null ou vide");
        }
        var obj = await _repository.GetAsync(x => x.ConsultationId == idConsultation, includes: z => z.Include(a => a.Diagnostics).Include(a => a.Treatments));
        return _mapper.Map<ConsultationsReadDto>(obj);
    }

    public async Task<object> GetAllConsultation()
    {
        var obj = await _repository.GetListAsync(includes: z => z.Include(a => a.Diagnostics).Include(a => a.Treatments));
        return _mapper.Map<IEnumerable<ConsultationsReadDto>>(obj);
    }

    public async Task<object> CreateConsultation(ConsultationsCreateDto entity)
    {
        if (entity.IdDme.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être null ou vide");
        }

        var itemToCreate = _mapper.Map<Consultations>(entity);
        await _repository.AddAsync(itemToCreate);
        return await _repository.Complete();
        
    }

    public async Task<object> UpdateConsultationById(Guid idConsultation, Delta<ConsultationsUpdateDto> entity)
    {
        if (idConsultation.IsNullOrEmpty() || entity ==null)
        {
            throw new Exception("L'id ne peut pas être null ou vide");
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

    public async Task<object> DeleteConsultationById(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être null ou vide");
        }

        var entity = await _repository.GetAsync(x => x.ConsultationId == idConsultation);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repository.Remove(entity);
        return await _repository.Complete();
    }
    #endregion
}
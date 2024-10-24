using AutoMapper;
using Common.Extension;
using Common.Services.Interfaces;
using Dme.Domain.Models;
using Dme.Dtos.Diagnostics;
using Dme.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Implementation;

public class DiagnosticService : IDiagnosticService
{
    public IRepository<Consultations> _consultationRepository { get; }
    private readonly IRepository<Diagnostics> _repositoryDiagnostic;
    private readonly IMapper _mapper;

    public DiagnosticService(IRepository<Diagnostics> repositoryDiagnostic,IRepository<Consultations> consultationRepository,IMapper mapper)
    {
        _consultationRepository = consultationRepository;
        _repositoryDiagnostic = repositoryDiagnostic;
        _mapper = mapper;
    }
    public async Task<object> GetDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic)
    {
        if (idConsultation.IsNullOrEmpty() || idDiagnostic.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _repositoryDiagnostic.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation  );
        return _mapper.Map<DiagnosticsReadDto>(obj);
    }

    public async Task<IEnumerable<DiagnosticsReadDto>> GetAllDiagnosticForConsultation(Guid idConsultation)
    {
        if (idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _repositoryDiagnostic.GetListAsync(a => a.FkIdConsultation == idConsultation);
        return _mapper.Map<IEnumerable<DiagnosticsReadDto>>(obj);
    }

    public async Task<object> CreateDiagnosticForConsultation(Guid idConsultation, DiagnosticsCreateDto entity)
    {
        if (entity == null || idConsultation.IsNullOrEmpty())
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var tmpConsultation = await _consultationRepository.GetAsync(a => a.ConsultationId == idConsultation);
        if (tmpConsultation == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var itemToCreate = _mapper.Map<Diagnostics>(entity);
        itemToCreate.FkIdConsultation = idConsultation;
        await _repositoryDiagnostic.AddAsync(itemToCreate);
        return await _repositoryDiagnostic.Complete();
    }

    public async Task<object> UpdateDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic, Delta<DiagnosticsUpdateDto> entity)
    {
        if (idConsultation.IsNullOrEmpty() || idConsultation.IsNullOrEmpty() || entity == null)
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entityToUpdate = await _repositoryDiagnostic.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation);
        if (entityToUpdate == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        var entityDto = _mapper.Map<DiagnosticsUpdateDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _repositoryDiagnostic.Complete();
    }

    public async Task<object> DeleteDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic)
    {
        if (idConsultation.IsNullOrEmpty() || idDiagnostic.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entity = await _repositoryDiagnostic.GetAsync(x => x.DiagnosticId == idDiagnostic || x.FkIdConsultation == idConsultation);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repositoryDiagnostic.Remove(entity);
        return await _repositoryDiagnostic.Complete();
    }
}
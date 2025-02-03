using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
using Dme.Application.DTOs.Diagnostics;
using Dme.Application.Interfaces;
using Dme.Domain.Models;
using Dme.Infrastructure.Persistence;

namespace Dme.Infrastructure.Implementation;

public class DiagnosticService : IDiagnosticService
{
    private readonly IRepository<Consultations, DmeDbContext> _consultationRepository;
    private readonly IRepository<Diagnostics, DmeDbContext> _repositoryDiagnostic;
    private readonly IMapper _mapper;

    public DiagnosticService(IRepository<Diagnostics, DmeDbContext> repositoryDiagnostic,IRepository<Consultations, DmeDbContext> consultationRepository,IMapper mapper)
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
        var result =  await _repositoryDiagnostic.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save the data to the database");
        }

        return true;
    }

    public async Task<object> PatchDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic, DiagnosticsPatchDto entity)
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
        entityToUpdate.UpdateWithDto(entity);
        var result = await _repositoryDiagnostic.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save the data to the database");
        }

        return true;
    }

    public async Task<object> DeleteDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic)
    {
        if (idConsultation.IsNullOrEmpty() || idDiagnostic.IsNullOrEmpty())
        {
            throw new Exception("Merci de vérifier les données saisie !");
        }
        var entity = await _repositoryDiagnostic.GetAsync(x => x.DiagnosticId == idDiagnostic && x.FkIdConsultation == idConsultation);
        if (entity == null)
        {
            throw new Exception($"Impossible de trouver l'entité à mettre à jour!");
        }
        _repositoryDiagnostic.Remove(entity);
        var result = await _repositoryDiagnostic.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save the data to the database");
        }

        return true;
    }
}
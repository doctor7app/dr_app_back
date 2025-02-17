using System.Text.Json;
using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Interfaces;
using Prescriptions.Domain.Event;
using Prescriptions.Domain.Models;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Implementation;

public class PrescriptionHistoryService : IPrescriptionHistoryService
{
    private readonly IRepository<Prescription, PrescriptionDbContext> _prescriptionRepo;
    private readonly IRepository<PrescriptionEvent, PrescriptionDbContext> _eventRepo;
    private readonly IMapper _mapper;

    public PrescriptionHistoryService(
        IRepository<Prescription, PrescriptionDbContext> prescriptionRepo,
        IRepository<PrescriptionEvent, PrescriptionDbContext> eventRepo,
        IMapper mapper)
    {
        _prescriptionRepo = prescriptionRepo;
        _eventRepo = eventRepo;
        _mapper = mapper;
    }

    public async Task<List<PrescriptionEventDto>> GetPrescriptionHistoryAsync(Guid prescriptionId)
    {
        var events = await _eventRepo.GetListAsync(e => e.FkPrescriptionId == prescriptionId,
            includes: e => e.Include(x => x.Prescription)
        );
        events = events.OrderBy(a => a.Timestamp);
        return _mapper.Map<List<PrescriptionEventDto>>(events);
    }

    public async Task RevertPrescriptionToVersionAsync(Guid prescriptionId, Guid eventId)
    {
        // 1. Récupérer l'événement cible
        var targetEvent = await _eventRepo.GetAsync(
            e => e.PrescriptionEventId == eventId && e.FkPrescriptionId == prescriptionId,
            includes: e => e.Include(x => x.Prescription)
        );

        if (targetEvent == null)
        {
            throw new KeyNotFoundException("Historical version not found");
        }

        // 2. Désérialiser l'état historique
        Prescription historicalState;
        try
        {
            historicalState = JsonSerializer.Deserialize<Prescription>(
                targetEvent.EventDataJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Invalid historical data format",ex.InnerException);
        }

        // 3. Récupérer la prescription actuelle
        var currentPrescription = await _prescriptionRepo.GetAsync(a=>a.PrescriptionId == prescriptionId);
        if (currentPrescription == null)
        {
            throw new KeyNotFoundException("Prescription not found");
        }

        // 4. Appliquer l'état historique
        _mapper.Map(historicalState, currentPrescription);

        // 5. Sauvegarder les changements
        await _prescriptionRepo.Complete();
        
    }
}
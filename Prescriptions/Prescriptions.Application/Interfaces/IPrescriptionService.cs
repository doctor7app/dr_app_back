﻿using Prescriptions.Application.Dtos.Events;
using Prescriptions.Application.Dtos.Prescriptions;

namespace Prescriptions.Application.Interfaces;

public interface IPrescriptionService
{
    Task<IEnumerable<PrescriptionDto>> GetAllPrescriptionAsync();
    Task<PrescriptionDto> GetPrescriptionByIdAsync(Guid id);
    Task<PrescriptionDetailsDto> GetPrescriptionDetailsAsync(Guid id);
    Task<bool> CreatePrescriptionAsync(PrescriptionCreateDto dto);
    Task<bool> UpdatePrescriptionAsync(PrescriptionUpdateDto dto);
    Task<bool> DeletePrescriptionAsync(Guid id);
    Task<bool> AddPrescriptionEventAsync(Guid prescriptionId, PrescriptionEventDto eventDto);
}
using AutoMapper;
using Common.Services.Interfaces;
using Microsoft.AspNetCore.OData.Deltas;
using Patients.Application.DTOs.MedicalInfo;
using Patients.Application.Interfaces;
using Patients.Domain.Models;
using Patients.Infrastructure.Persistence;

namespace Patients.Infrastructure.Implementation;

public class MedicalInfoService : IMedicalInfoService
{
    private readonly IRepository<MedicalInformation, PatientDbContext> _work;
    private readonly IMapper _mapper;
    public MedicalInfoService(IRepository<MedicalInformation, PatientDbContext> work, IMapper mapper)
    {
        _work = work;
        _mapper = mapper;
    }


    public async Task<object> GetMedicalInfo(Guid patientId, Guid medicalInfoId)
    {
        if (patientId == Guid.Empty || medicalInfoId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.MedicalInformationId == medicalInfoId);
        return _mapper.Map<MedicalInfoDto>(obj);
    }

    public async Task<IEnumerable<MedicalInfoDto>> GetRelative(Guid patientId)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var obj = await _work.GetListAsync(x => x.FkIdPatient == patientId);
        return _mapper.Map<IEnumerable<MedicalInfoDto>>(obj);
    }

    public async Task<object> Create(Guid patientId, MedicalInfoCreateDto entity)
    {
        if (patientId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        entity.PatientId = patientId;
        var item = _mapper.Map<MedicalInformation>(entity);
        await _work.AddAsync(item);
        return await _work.Complete();
    }

    public async Task<object> Update(Guid patientId, Guid medicalInfoId, Delta<MedicalInfoDto> entity)
    {
        if (patientId == Guid.Empty || medicalInfoId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var entityToUpdate = await _work.GetAsync(z => z.MedicalInformationId == medicalInfoId && z.FkIdPatient == patientId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!");
        }
        var entityDto = _mapper.Map<MedicalInfoDto>(entityToUpdate);
        entity.Patch(entityDto);
        _mapper.Map(entityDto, entityToUpdate);
        return await _work.Complete();
    }

    public async Task<object> Delete(Guid patientId, Guid medicalInfoId)
    {
        if (patientId == Guid.Empty || medicalInfoId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.MedicalInformationId == medicalInfoId);
        _work.Remove(obj);
        return await _work.Complete();
    }
}
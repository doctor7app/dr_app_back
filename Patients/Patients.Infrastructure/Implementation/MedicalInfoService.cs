using AutoMapper;
using Common.Extension.Common;
using Common.Services.Interfaces;
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
        
        var item = _mapper.Map<MedicalInformation>(entity);
        item.FkIdPatient = patientId;
        await _work.AddAsync(item);
        var res =  await _work.Complete() > 0;
        if (!res)
        {
            throw new Exception("Could not save data to database");
        }

        return true;

    }

    public async Task<object> Patch(Guid patientId, Guid medicalInfoId, MedicalInfoPatchDto entity)
    {
        if (patientId == Guid.Empty || medicalInfoId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }

        var entityToUpdate = await _work.GetAsync(z => z.MedicalInformationId == medicalInfoId 
                                                       && z.FkIdPatient == patientId);
        if (entityToUpdate == null)
        {
            throw new Exception($"L'élement avec l'id {medicalInfoId} n'existe pas dans la base de données!");
        }
        entityToUpdate.UpdateWithDto(entity);
        var result = await _work.Complete() > 0;
        if (!result)
        {
            throw new Exception("Could not save data to the database");
        }

        return true;
    }

    public async Task<object> Delete(Guid patientId, Guid medicalInfoId)
    {
        if (patientId == Guid.Empty || medicalInfoId == Guid.Empty)
        {
            throw new Exception("L'id ne peut pas être un Guid Vide");
        }
        var obj = await _work.GetAsync(x => x.FkIdPatient == patientId && x.MedicalInformationId == medicalInfoId);
        _work.Remove(obj);
        var res =  await _work.Complete() >0;
        if (!res)
        {
            throw new Exception("Could not save data to the database");
        }

        return true;
    }
}
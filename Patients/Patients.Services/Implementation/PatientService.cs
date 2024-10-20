using AutoMapper;
using Common.Services.Implementation;
using Common.Services.Interfaces;
using Patients.Domain.Models;
using Patients.Dtos.Classes.Patient;
using Patients.Services.Interfaces;

namespace Patients.Services.Implementation;

public class PatientService : ServiceGeneric<Patient,PatientDto, PatientCreateDto, PatientUpdateDto>,IPatientService
{
    private readonly IRepository<Patient> _work;
    private readonly IMapper _mapper;
    public PatientService(IRepository<Patient> work, IMapper mapper) : base(work, mapper)
    {
        _work = work;
        _mapper = mapper;
    }
}
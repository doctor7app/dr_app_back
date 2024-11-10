using AutoMapper;
using Common.Services.Implementation;
using Common.Services.Interfaces;
using Patients.Application.DTOs.Patient;
using Patients.Application.Interfaces;
using Patients.Domain.Models;

namespace Patients.Infrastructure.Implementation;

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
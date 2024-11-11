using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Patients.Domain.Models;

namespace Patients.Application.DTOs.MedicalInfo;

public class MedicalInfoCreateDto : IMapFrom<MedicalInformation>
{
    [Required]
    public MedicalInformationType Type { get; set; }
    [Required]
    public string Name { get; set; }
    public string Note { get; set; }
    [Required(ErrorMessage = "Patient Id is required to create the adresse")]
    public Guid PatientId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<MedicalInfoCreateDto, MedicalInformation>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.FkIdPatient, opt => opt.MapFrom(src => src.PatientId))
            ;
    }
}
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Patients.Domain.Models;

namespace Patients.Application.DTOs.MedicalInfo;

public class MedicalInfoPatchDto : IMapFrom<MedicalInformation>
{
    public MedicalInformationType Type { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<MedicalInformation, MedicalInfoPatchDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ReverseMap()
            ;
    }
}
﻿using AutoMapper;
using Common.Enums;
using Patients.Domain.Models;
using Patients.Dtos.Interfaces;

namespace Patients.Dtos.Classes.MedicalInfo;

public class MedicalInfoDto :IMapFrom<MedicalInformation>
{
    public Guid Id { get; set; }
    public MedicalInformationType Type { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<MedicalInformation, MedicalInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.MedicalInformationId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ReverseMap()
            ;
    }
}
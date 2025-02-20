﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Extension.DataAnnotations;
using Common.Interfaces;

namespace Dme.Application.DTOs.Dmes;

public class DmeCreateDto : IMapFrom<Domain.Models.Dme>
{
    public string Notes { get; set; }
    public string AdditionalInformations { get; set; }
    [Required(ErrorMessage = "Patient is required")]
    [NotEmptyGuid(ErrorMessage = "The Id cannot be null or empty.")]
    public Guid PatientId { get; set; }
    [Required(ErrorMessage = "Doctor is required")]
    [NotEmptyGuid(ErrorMessage = "The Id cannot be null or empty.")]
    public Guid DoctorId { get; set; }

    public PatientState State { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DmeCreateDto, Domain.Models.Dme>()
            .ForMember(dest => dest.FkIdPatient, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.FkIdDoctor, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.AdditionalInformations, opt => opt.MapFrom(src => src.AdditionalInformations))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            ;
    }
}
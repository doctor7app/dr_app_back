﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Interfaces;

namespace Dme.Application.DTOs.Diagnostics;

public class DiagnosticsPatchDto : IMapFrom<Domain.Models.Diagnostics>
{
    public string TypeDiagnostic { get; set; }
    public string Description { get; set; }
    public string Results { get; set; }
    public string Comments { get; set; }

    [Required(ErrorMessage = "Consultation is required")]
    public Guid ConsultationId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DiagnosticsPatchDto,Domain.Models.Diagnostics>()
            .ForMember(dest => dest.FkIdConsultation, opt => opt.MapFrom(src => src.ConsultationId))
            .ForMember(dest => dest.TypeDiagnostic, opt => opt.MapFrom(src => src.TypeDiagnostic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Results))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            ;
    }
}
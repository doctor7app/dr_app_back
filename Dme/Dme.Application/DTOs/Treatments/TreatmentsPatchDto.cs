﻿using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Interfaces;

namespace Dme.Application.DTOs.Treatments;

public class TreatmentsPatchDto : IMapFrom<Domain.Models.Treatments>
{
    public string Medicament { get; set; }
    public string Dose { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }

    [Required(ErrorMessage = "Consultation is required")]
    public Guid ConsultationId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TreatmentsPatchDto,Domain.Models.Treatments>()
            .ForMember(dest => dest.FkIdConsultation, opt => opt.MapFrom(src => src.ConsultationId))
            .ForMember(dest => dest.Medicament, opt => opt.MapFrom(src => src.Medicament))
            .ForMember(dest => dest.Dose, opt => opt.MapFrom(src => src.Dose))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            ;
    }
}
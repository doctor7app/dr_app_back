using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Dme.Application.DTOs.Diagnostics;
using Dme.Application.DTOs.Treatments;

namespace Dme.Application.DTOs.Consultations;

public class ConsultationsReadDto : IMapFrom<Domain.Models.Consultations>
{
    [Key]
    public Guid Id { get; set; }
    public string ReasonOfVisit { get; set; }
    public string Symptoms { get; set; }
    public decimal Weight { get; set; }
    public decimal Height { get; set; }
    public string PressureArterial { get; set; }
    public decimal Temperature { get; set; }
    public int CardiacFrequency { get; set; }
    public decimal SaturationOxygen { get; set; }
    public DateTime ConsultationDate { get; set; }
    public DateTime? NextConsultationDate { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationState State { get; set; }
    public Guid IdDme { get; set; }

    public DateTime Created { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid LastModifiedById { get; set; }


    public ICollection<DiagnosticsReadDto> Diagnostics { get; set; }
    public ICollection<TreatmentsReadDto> Treatments { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Consultations, ConsultationsReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ConsultationId))
            .ForMember(dest => dest.IdDme, opt => opt.MapFrom(src => src.FkIdDme))
            .ForMember(dest => dest.ReasonOfVisit, opt => opt.MapFrom(src => src.ReasonOfVisit))
            .ForMember(dest => dest.Symptoms, opt => opt.MapFrom(src => src.Symptoms))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
            .ForMember(dest => dest.PressureArterial, opt => opt.MapFrom(src => src.PressureArterial))
            .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
            .ForMember(dest => dest.CardiacFrequency, opt => opt.MapFrom(src => src.CardiacFrequency))
            .ForMember(dest => dest.SaturationOxygen, opt => opt.MapFrom(src => src.SaturationOxygen))
            .ForMember(dest => dest.ConsultationDate, opt => opt.MapFrom(src => src.ConsultationDate))
            .ForMember(dest => dest.NextConsultationDate, opt => opt.MapFrom(src => src.NextConsultationDate))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.Diagnostics, opt => opt.MapFrom(src => src.Diagnostics))
            .ForMember(dest => dest.Treatments, opt => opt.MapFrom(src => src.Treatments))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
            .ForMember(dest => dest.LastModifiedById, opt => opt.MapFrom(src => src.LastModifiedById))
            .ReverseMap()
            ;
    }
}
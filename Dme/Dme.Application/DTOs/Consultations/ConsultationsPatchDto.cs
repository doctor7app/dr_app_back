using AutoMapper;
using Common.Enums;
using Common.Extension.DataAnnotations;
using Common.Interfaces;

namespace Dme.Application.DTOs.Consultations;

public class ConsultationsPatchDto : IMapFrom<Domain.Models.Consultations>
{
    public string ReasonOfVisit { get; set; }
    public string Symptoms { get; set; }
    [PositiveDecimal(ErrorMessage = "Weight must be a positive value.")]
    public decimal? Weight { get; set; }
    [PositiveDecimal(ErrorMessage = "Height must be a positive value.")]
    public decimal? Height { get; set; }
    public string PressureArterial { get; set; }
    [PositiveDecimal(ErrorMessage = "Temperature must be a positive value.")]
    public decimal? Temperature { get; set; }
    [PositiveInt(ErrorMessage = "Cardiac Frequency must be a positive value.")]
    public int CardiacFrequency { get; set; }
    [PositiveDecimal(ErrorMessage = "Saturation Oxygen must be a positive value.")]
    public decimal? SaturationOxygen { get; set; }
    [DateGreaterThanOrEqualToToday(ErrorMessage = "The appointment date cannot be in the past.")]
    public DateTime ConsultationDate { get; set; }
    public DateTime? NextConsultationDate { get; set; }
    public ConsultationType Type { get; set; }
    public ConsultationState State { get; set; }
    

    public void Mapping(Profile profile)
    {
        profile.CreateMap < ConsultationsPatchDto,Domain.Models.Consultations>()
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
            ;
    }
}
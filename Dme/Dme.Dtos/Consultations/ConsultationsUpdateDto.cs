using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Dme.Dtos.Consultations;

public class ConsultationsUpdateDto : IMapFrom<Domain.Models.Consultations>
{
    [Key]
    [Required(ErrorMessage = "Consultation id is required")]
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
    

    public void Mapping(Profile profile)
    {
        profile.CreateMap < ConsultationsUpdateDto,Domain.Models.Consultations>()
            .ForMember(dest => dest.ConsultationId, opt => opt.MapFrom(src => src.Id))
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
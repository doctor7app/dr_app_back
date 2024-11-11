using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Interfaces;

namespace Dme.Application.DTOs.Treatments;

public class TreatmentsReadDto : IMapFrom<Domain.Models.Treatments>
{
    [Key]
    public Guid Id { get; set; }
    public string Medicament { get; set; }
    public string Dose { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }
    
    public Guid ConsultationId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Treatments, TreatmentsReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TreatmentsId))
            .ForMember(dest => dest.ConsultationId, opt => opt.MapFrom(src => src.FkIdConsultation))
            .ForMember(dest => dest.Medicament, opt => opt.MapFrom(src => src.Medicament))
            .ForMember(dest => dest.Dose, opt => opt.MapFrom(src => src.Dose))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            .ReverseMap()
            ;
    }
}
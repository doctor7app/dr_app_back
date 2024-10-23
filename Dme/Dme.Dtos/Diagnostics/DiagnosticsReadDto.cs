using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Interfaces;

namespace Dme.Dtos.Diagnostics;

public class DiagnosticsReadDto : IMapFrom<Domain.Models.Diagnostics>
{
    [Key]
    public Guid Id { get; set; }
    public string TypeDiagnostic { get; set; }
    public string Description { get; set; }
    public string Results { get; set; }
    public string Comments { get; set; }

    public Guid ConsultationId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Diagnostics, DiagnosticsReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DiagnosticId))
            .ForMember(dest => dest.TypeDiagnostic, opt => opt.MapFrom(src => src.TypeDiagnostic))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Results))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.ConsultationId, opt => opt.MapFrom(src => src.FkIdConsultation))
            .ReverseMap()
            ;
    }
}
using AutoMapper;
using Common.Interfaces;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Items;

public class PrescriptionItemUpdateDto : IMapFrom<PrescriptionItem>
{
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }
    public bool? IsPrn { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionItemUpdateDto, PrescriptionItem>()
            .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            .ForMember(dest => dest.IsPrn, opt => opt.MapFrom(src => src.IsPrn))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))
            ;
    }
}
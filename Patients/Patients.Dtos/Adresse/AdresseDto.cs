using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Patients.Dtos.Adresse;

public class AdresseDto : IMapFrom<Domain.Models.Adresse>
{
    [Key]
    public Guid Id { get; set; }
    public string Country { get; set; }
    public string Provence { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Street { get; set; }
    public string AdditionalInformation { get; set; }
    public AdresseType Type { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Adresse, AdresseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AdresseId))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Provence, opt => opt.MapFrom(src => src.Provence))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.AdditionalInformation))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ReverseMap()
            ;
    }
}
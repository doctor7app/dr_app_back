using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Patients.Application.DTOs.Adresse;

public class AdressePatchDto : IMapFrom<Domain.Models.Adresse>
{
    [Required(ErrorMessage = "Country is required")]
    [MinLength(1,ErrorMessage = "Country should have a value")]
    public string Country { get; set; }
    [Required(ErrorMessage = "Provence is required")]
    [MinLength(1, ErrorMessage = "Provence should have a value")]
    public string Provence { get; set; }
    [Required(ErrorMessage = "City is required")]
    [MinLength(1, ErrorMessage = "City should have a value")]
    public string City { get; set; }
    [Required(ErrorMessage = "PostalCode is required")]
    [MinLength(1, ErrorMessage = "PostalCode should have a value")]
    public string PostalCode { get; set; }
    [Required(ErrorMessage = "Street is required")]
    [MinLength(1, ErrorMessage = "Street should have a value")]
    public string Street { get; set; }
    public string AdditionalInformation { get; set; }
    public AdresseType Type { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Adresse, AdressePatchDto>()
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
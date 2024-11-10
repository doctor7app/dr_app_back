using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Patients.Application.DTOs.Adresse;

public class AdresseCreateDto : IMapFrom<Domain.Models.Adresse>
{
    [Required]
    public string Country { get; set; }
    [Required]
    public string Provence { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    public string PostalCode { get; set; }
    [Required]
    public string Street { get; set; }
    public string AdditionalInformation { get; set; }
    public AdresseType Type { get; set; }

    [Required(ErrorMessage = "Patient Id is required to create the adresse")]
    public Guid IdPatient { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AdresseCreateDto, Domain.Models.Adresse>()

            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Provence, opt => opt.MapFrom(src => src.Provence))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
            .ForMember(dest => dest.AdditionalInformation, opt => opt.MapFrom(src => src.AdditionalInformation))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.FkIdPatient, opt => opt.MapFrom(src => src.IdPatient))
            ;
    }
}
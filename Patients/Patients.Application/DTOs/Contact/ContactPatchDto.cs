using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Patients.Application.DTOs.Contact;

public class ContactPatchDto : IMapFrom<Domain.Models.Contact>
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public ContactType Type { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Contact, ContactPatchDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ReverseMap()
            ;
    }
}
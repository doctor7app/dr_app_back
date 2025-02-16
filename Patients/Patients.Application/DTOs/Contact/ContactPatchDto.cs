using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;

namespace Patients.Application.DTOs.Contact;

public class ContactPatchDto : IMapFrom<Domain.Models.Contact>
{
    [Required(ErrorMessage = "First Name is required"), MinLength(1, ErrorMessage = "First Name can not be empty.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Last Name is required"), MinLength(1, ErrorMessage = "Last Name can not be empty.")]
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    [Required(ErrorMessage = "Contact Type is required")]
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
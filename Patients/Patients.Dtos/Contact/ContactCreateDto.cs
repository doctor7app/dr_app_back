using AutoMapper;
using Common.Enums;
using System.ComponentModel.DataAnnotations;
using Common.Interfaces;

namespace Patients.Dtos.Contact;

public class ContactCreateDto : IMapFrom<Domain.Models.Contact>
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }

    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    [Required]
    public ContactType Type { get; set; }
    [Required(ErrorMessage = "Patient Id is required to create the adresse")]
    public Guid IdPatient { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ContactCreateDto, Domain.Models.Contact>()
            .ForMember(dest => dest.FkIdPatient, opt => opt.MapFrom(src => src.IdPatient))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            ;
    }
}
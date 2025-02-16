using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;
using Common.Enums.Patients;
using Common.Interfaces;

namespace Patients.Application.DTOs.Patient;

public class PatientCreateDto : IMapFrom<Domain.Models.Patient>
{
    [MaxLength(80, ErrorMessage = "SocialNumber must not exceed 80 characters")]
    [JsonPropertyName("socialNumber")]
    public string SocialNumber { get; set; }
    [Required(ErrorMessage = "First Name is Required"), MinLength(1, ErrorMessage = "First Name can not be empty.")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Last Name is Required"), MinLength(1, ErrorMessage = "Last Name can not be empty.")]
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string HomeNumber { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PatientCreateDto, Domain.Models.Patient>()
            .ForMember(dest => dest.SocialSecurityNumber, opt => opt.MapFrom(src => src.SocialNumber))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.DeathDate, opt => opt.MapFrom(src => src.DeathDate))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.HomeNumber))
            ;
    }
}
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Patients.Application.DTOs.Adresse;
using Patients.Application.DTOs.Contact;
using Patients.Application.DTOs.MedicalInfo;

namespace Patients.Application.DTOs.Patient;

public class PatientDto : IMapFrom<Domain.Models.Patient>
{
    [Key]
    public Guid Id { get; set; }
    public string SocialNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime DeathDate { get; set; }
    public Gender Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string HomeNumber { get; set; }
    public int Age { get; set; }

    public IEnumerable<ContactDto> Contacts { get; set; }
    public IEnumerable<AdresseDto> Adresses { get; set; }
    public IEnumerable<MedicalInfoDto> MedicalInfos { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Patient, PatientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.SocialNumber, opt => opt.MapFrom(src => src.SocialSecurityNumber))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))

            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.DeathDate, opt => opt.MapFrom(src => src.DeathDate))

            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.HomeNumber))

            .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>

                 DateTime.Today.Year - src.BirthDate.Year
             ))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts))
            .ForMember(dest => dest.Adresses, opt => opt.MapFrom(src => src.Adresses))
            .ForMember(dest => dest.MedicalInfos, opt => opt.MapFrom(src => src.MedicalInformations))
            ;
    }
}
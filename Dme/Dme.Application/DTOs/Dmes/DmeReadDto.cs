using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Dme.Application.DTOs.Consultations;

namespace Dme.Application.DTOs.Dmes;

public class DmeReadDto :IMapFrom<Domain.Models.Dme>
{
    [Key]
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public string AdditionalInformations { get; set; }

    public string PatientName { get; set; }
    //Must be unique in the application, one patient can have one dme only.
    public Guid PatientId { get; set; }
    //Can be removed.
    public string DoctorName { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime Created { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastModified { get; set; }
    public Guid LastModifiedById { get; set; }

    public PatientState State { get; set; }

    public IEnumerable<ConsultationsReadDto> Consultations { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Models.Dme, DmeReadDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DmeId))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.FkIdPatient))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.FkIdDoctor))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.AdditionalInformations, opt => opt.MapFrom(src => src.AdditionalInformations))
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
            .ForMember(dest => dest.LastModifiedById, opt => opt.MapFrom(src => src.LastModifiedById))
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
            .ForMember(dest => dest.Consultations, opt => opt.MapFrom(src => src.Consultations))
            .ReverseMap()
            ;
    }
}
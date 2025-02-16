using AutoMapper;
using Common.Interfaces;
using Prescriptions.Application.Dtos.Prescriptions;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Events;

public class PrescriptionDetailsDto : PrescriptionDto, IMapFrom<Prescription>
{
    public List<PrescriptionEventDto> Events { get; set; }

    public new void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionDetailsDto, Prescription>()
            .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.IssuedAt))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ConsultationType, opt => opt.MapFrom(src => src.ConsultationType))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.FkPatientId, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.FkConsultationId, opt => opt.MapFrom(src => src.ConsultationId))
            .ForMember(dest => dest.FkDoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.DomainEvents, opt => opt.MapFrom(src => src.Events))
            .ReverseMap()
            ;
    }
}
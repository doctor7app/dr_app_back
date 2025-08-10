using AutoMapper;
using Common.Enums;
using Common.Enums.Prescriptions;
using Common.Interfaces;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Prescriptions;

public class PrescriptionDto : IMapFrom<Prescription>
{
    public Guid Id { get; set; }
    public DateTime IssuedAt { get; set; }
    public string Notes { get; set; }
    public PrescriptionStatus Status { get; set; }
    public ConsultationType ConsultationType { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public Guid PatientId { get; set; }
    public Guid ConsultationId { get; set; }
    public Guid DoctorId { get; set; }

    public List<PrescriptionItemDto> Items { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionDto, Prescription>()
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
            .ReverseMap()
            ;
    }
}
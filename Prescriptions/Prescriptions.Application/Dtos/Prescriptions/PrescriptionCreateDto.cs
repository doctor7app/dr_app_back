using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Prescriptions.Application.Dtos.Items;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Prescriptions;

public class PrescriptionCreateDto : IMapFrom<Prescription>
{
    public string Notes { get; set; }
    public ConsultationType ConsultationType { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public Guid PatientId { get; set; }
    public Guid ConsultationId { get; set; }
    public Guid DoctorId { get; set; }

    public List<PrescriptionItemCreateDto> Items { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionCreateDto, Prescription>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.ConsultationType, opt => opt.MapFrom(src => src.ConsultationType))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.FkPatientId, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.FkConsultationId, opt => opt.MapFrom(src => src.ConsultationId))
            .ForMember(dest => dest.FkDoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            ;
    }
}
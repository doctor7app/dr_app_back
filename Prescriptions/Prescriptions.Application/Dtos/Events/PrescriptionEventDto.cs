using AutoMapper;
using Common.Enums.Prescriptions;
using Common.Interfaces;
using Prescriptions.Domain.Event;

namespace Prescriptions.Application.Dtos.Events;

public class PrescriptionEventDto : IMapFrom<PrescriptionEvent>
{
    public Guid Id { get; set; }
    public PrescriptionEventType EventType { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DoctorId { get; set; }
    public string EventDataJson { get; set; }

    public Guid PrescriptionId { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionEventDto, PrescriptionEvent>()
            .ForMember(dest => dest.PrescriptionEventId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => src.EventType))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.EventDataJson, opt => opt.MapFrom(src => src.EventDataJson))
            .ForMember(dest => dest.FkPrescriptionId, opt => opt.MapFrom(src => src.PrescriptionId))
            .ReverseMap()
            ;
    }
}
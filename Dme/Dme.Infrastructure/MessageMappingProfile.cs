using Contracts.Messages.Consultations;
using Contracts.Messages.Dmes;
using Dme.Application.DTOs.Consultations;
using Dme.Application.DTOs.Dmes;

namespace Dme.Infrastructure
{
    public class MessageMappingProfile : AutoMapper.Profile
    {
        public MessageMappingProfile()
        {
            #region Dme Messages

            CreateMap<DmeReadDto,DmeCreatedEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.AdditionalInformations, opt => opt.MapFrom(src => src.AdditionalInformations))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModified))
                .ForMember(dest => dest.LastModifiedById, opt => opt.MapFrom(src => src.LastModifiedById))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                ;

            CreateMap<DmePatchDto,DmeUpdatedEvent>()
                // Need to Get the Id
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.AdditionalInformations, opt => opt.MapFrom(src => src.AdditionalInformations))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                ;

            CreateMap<ConsultationsReadDto, ConsultationCreatedEvent>()
                .ForMember(dest=>dest.Id,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest => dest.IdDme, opt => opt.MapFrom(src => src.IdDme))
                .ForMember(dest => dest.ReasonOfVisit, opt => opt.MapFrom(src => src.ReasonOfVisit))
                .ForMember(dest => dest.Symptoms, opt => opt.MapFrom(src => src.Symptoms))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.PressureArterial, opt => opt.MapFrom(src => src.PressureArterial))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
                .ForMember(dest => dest.CardiacFrequency, opt => opt.MapFrom(src => src.CardiacFrequency))
                .ForMember(dest => dest.SaturationOxygen, opt => opt.MapFrom(src => src.SaturationOxygen))
                .ForMember(dest => dest.ConsultationDate, opt => opt.MapFrom(src => src.ConsultationDate))
                .ForMember(dest => dest.NextConsultationDate, opt => opt.MapFrom(src => src.NextConsultationDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ReverseMap()
                ;

            CreateMap<ConsultationsPatchDto, ConsultationUpdatedEvent>()
                // Need to get the ID
                .ForMember(dest => dest.ReasonOfVisit, opt => opt.MapFrom(src => src.ReasonOfVisit))
                .ForMember(dest => dest.Symptoms, opt => opt.MapFrom(src => src.Symptoms))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.PressureArterial, opt => opt.MapFrom(src => src.PressureArterial))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
                .ForMember(dest => dest.CardiacFrequency, opt => opt.MapFrom(src => src.CardiacFrequency))
                .ForMember(dest => dest.SaturationOxygen, opt => opt.MapFrom(src => src.SaturationOxygen))
                .ForMember(dest => dest.ConsultationDate, opt => opt.MapFrom(src => src.ConsultationDate))
                .ForMember(dest => dest.NextConsultationDate, opt => opt.MapFrom(src => src.NextConsultationDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ReverseMap()
                ;

            #endregion
        }
    }
}
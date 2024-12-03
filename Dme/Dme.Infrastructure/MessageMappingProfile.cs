using Contracts.Messages.Dmes;
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
                //.ForMember(dest => dest.Consultations, opt => opt.MapFrom(src => src.Consultations))
                ;

            CreateMap<DmePatchDto,DmeUpdatedEvent>()
                // Need to Get the Id
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.AdditionalInformations, opt => opt.MapFrom(src => src.AdditionalInformations))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                ;

            #endregion
        }
    }
}
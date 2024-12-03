﻿using Contracts.Messages.Patients;
using Patients.Application.DTOs.Patient;

namespace Patients.Infrastructure
{
    public class MessageMappingProfile : AutoMapper.Profile
    {
        public MessageMappingProfile()
        {
            #region Patient Messages

            CreateMap<PatientCreatedEvent, PatientDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SocialNumber, opt => opt.MapFrom(src => src.SocialNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.DeathDate, opt => opt.MapFrom(src => src.DeathDate))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.HomeNumber))
                .ReverseMap();

            CreateMap<PatientUpdatedEvent, PatientDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SocialNumber, opt => opt.MapFrom(src => src.SocialNumber))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.DeathDate, opt => opt.MapFrom(src => src.DeathDate))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.HomeNumber))
                .ReverseMap();

            //CreateMap<PatientDeletedEvent, Patient>()
            //    .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Id))
            //    .ReverseMap();

            #endregion

        }
    }
}
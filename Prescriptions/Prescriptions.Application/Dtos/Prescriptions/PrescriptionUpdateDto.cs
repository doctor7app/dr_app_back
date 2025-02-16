using AutoMapper;
using Common.Enums.Prescriptions;
using Common.Interfaces;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Prescriptions;

public class PrescriptionUpdateDto : IMapFrom<Prescription>
{
    public Guid Id { get; set; }
    public string Notes { get; set; }
    public PrescriptionStatus Status { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionUpdateDto, Prescription>()
            .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            ;
    }
}
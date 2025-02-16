using AutoMapper;
using Common.Enums.Prescriptions;
using Common.Interfaces;
using Prescriptions.Domain.Models;

namespace Prescriptions.Application.Dtos.Items;

public class PrescriptionItemDto : IMapFrom<PrescriptionItem>
{
    public Guid Id { get; set; }
    public string DrugName { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }
    public MedicationType MedicationType { get; set; }
    public bool IsEssential { get; set; }
    public AdministrationRoute Route { get; set; }
    public string TimeOfDay { get; set; }
    public string MealInstructions { get; set; }
    public bool IsPrn { get; set; }
    public string Notes { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<PrescriptionItemDto, PrescriptionItem>()
            .ForMember(dest => dest.PrescriptionItemId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DrugName, opt => opt.MapFrom(src => src.DrugName))
            .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
            .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Instructions))
            .ForMember(dest => dest.MedicationType, opt => opt.MapFrom(src => src.MedicationType))
            .ForMember(dest => dest.IsEssential, opt => opt.MapFrom(src => src.IsEssential))
            .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route))
            .ForMember(dest => dest.TimeOfDay, opt => opt.MapFrom(src => src.TimeOfDay))
            .ForMember(dest => dest.MealInstructions, opt => opt.MapFrom(src => src.MealInstructions))
            .ForMember(dest => dest.IsPrn, opt => opt.MapFrom(src => src.IsPrn))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ReverseMap()
            ;
    }
}
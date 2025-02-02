using AutoMapper;
using Common.Contracts.Notif;
using Notify.Core.Models;

namespace Notify.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<NotifRequest,NotifItem>();
    }
}

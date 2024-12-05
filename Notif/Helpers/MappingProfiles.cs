using AutoMapper;
using Common.Contracts.Notif;
using Notif.Core.Models;

namespace Notif.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<NotifItem, NotifRequest>();
    }
}

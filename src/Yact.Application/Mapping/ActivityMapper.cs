using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Activity;

namespace Yact.Application.Mapping;

public class ActivityMapper : Profile
{
    public ActivityMapper()
    {
        CreateMap<Activity, ActivityInfoDto>();
        CreateMap<ActivityInfoDto, Activity>();
    }
}

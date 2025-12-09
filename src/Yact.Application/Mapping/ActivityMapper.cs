using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Activity;

namespace Yact.Application.Mapping;

public class ActivityMapper : Profile
{
    public ActivityMapper()
    {
        CreateMap<ActivityInfo, ActivityInfoDto>();
        CreateMap<ActivityInfoDto, ActivityInfo>();
    }
}

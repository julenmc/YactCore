using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Climb;

namespace Yact.Application.Mapping;

public class ActivityClimbMapper : Profile
{
    public ActivityClimbMapper()
    {
        CreateMap<ActivityClimb, ActivityClimbDto>();
        CreateMap<ActivityClimbDto, ActivityClimb>();
    }
}

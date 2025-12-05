using AutoMapper;
using Yact.Application.DTOs;
using Yact.Domain.Models;

namespace Yact.Application.Mapping;

public class ActivityMapper
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Activity, ActivityDto>();
            config.CreateMap<ActivityDto, Activity>();
        });
        return mappingConfig;
    }
}

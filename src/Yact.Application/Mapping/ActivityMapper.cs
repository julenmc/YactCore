using AutoMapper;
using Yact.Application.DTOs;
using Yact.Domain.Entities.Activity;

namespace Yact.Application.Mapping;

public class ActivityMapper
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<ActivityInfo, ActivityInfoDto>();
            config.CreateMap<ActivityInfoDto, ActivityInfo>();
        });
        return mappingConfig;
    }
}

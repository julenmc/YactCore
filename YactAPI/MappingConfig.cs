using AutoMapper;
using YactAPI.Models;
using YactAPI.Models.Dto;

namespace YactAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ClimbDto, Climb>();
                config.CreateMap<Climb, ClimbDto>();
                config.CreateMap<ActivityDto, Activity>();
                config.CreateMap<Activity, ActivityDto>();
                config.CreateMap<ActivityPointDto, ActivityPoint>();
                config.CreateMap<ActivityPoint, ActivityPointDto>();
            });
            return mappingConfig;
        }
    }
}

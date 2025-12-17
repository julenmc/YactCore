using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Cyclist;

namespace Yact.Application.Mapping;

public class CyclistMapper : Profile
{
    public CyclistMapper()
    {
        CreateMap<Zone, ZoneDto>();
        CreateMap<ZoneDto, Zone>();

        CreateMap<PowerCurve, PowerCurveDto>();
        CreateMap<PowerCurveDto, PowerCurve>();

        CreateMap<CyclistFitness, CyclistFitnessDto>()
            .ForMember(dest => dest.HrZones,
                opt => opt.MapFrom(src => src.HrZones!.ToDictionary(k => $"zone{k.Key}", v => v.Value)))
            .ForMember(dest => dest.PowerZones,
                opt => opt.MapFrom(src => src.PowerZones!.ToDictionary(k => $"zone{k.Key}", v => v.Value)));
        CreateMap<CyclistFitnessDto, CyclistFitness>()
            .ForMember(dest => dest.HrZones,
                opt => opt.MapFrom(src => src.HrZones!.ToDictionary(k => Int16.Parse(k.Key.Replace("zone", "")), v => v.Value)))
            .ForMember(dest => dest.PowerZones,
                opt => opt.MapFrom(src => src.PowerZones!.ToDictionary(k => Int16.Parse(k.Key.Replace("zone", "")), v => v.Value)));

        CreateMap<Cyclist, CyclistDto>();
        CreateMap<CyclistDto, Cyclist>();
    }
}

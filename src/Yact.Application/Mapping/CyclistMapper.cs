using AutoMapper;
using Domain = Yact.Domain;

namespace Yact.Application.Mapping;

public class CyclistMapper : Profile
{
    public CyclistMapper()
    {
        CreateMap<Domain.ValueObjects.Cyclist.Zone, Common.Zone>();
        CreateMap<Domain.ValueObjects.Common.PowerCurve, Responses.PowerCurveResponse>();
        CreateMap<Domain.Entities.CyclistFitness, Responses.CyclistFitnessResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Height,
                opt => opt.MapFrom(src => src.Size.HeightCm))
            .ForMember(dest => dest.Weight,
                opt => opt.MapFrom(src => src.Size.WeightKg))
            .ForMember(dest => dest.HrZones,
                opt => opt.MapFrom(src => src.HrZones!.ToDictionary(k => $"zone{k.Key}", v => v.Value)))
            .ForMember(dest => dest.PowerZones,
                opt => opt.MapFrom(src => src.PowerZones!.ToDictionary(k => $"zone{k.Key}", v => v.Value)));
        CreateMap<Domain.Entities.Cyclist, Responses.CyclistResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.FitnessHistory,
                opt => opt.MapFrom(src => src.FitnessHistory));
    }
}

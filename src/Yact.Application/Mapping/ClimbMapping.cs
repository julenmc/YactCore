using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities;

namespace Yact.Application.Mapping;

public class ClimbMapping : Profile
{
    public ClimbMapping()
    {
        CreateMap<Climb, ClimbResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id.Value))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Summary.Name))
            .ForMember(dest => dest.DistanceMeters,
                opt => opt.MapFrom(src => src.Data.Metrics.DistanceMeters))
            .ForMember(dest => dest.Slope,
                opt => opt.MapFrom(src => src.Data.Metrics.Slope))
            .ForMember(dest => dest.MaxSlope,
                opt => opt.MapFrom(src => src.Data.Metrics.MaxSlope))
            .ForMember(dest => dest.NetElevationMeters,
                opt => opt.MapFrom(src => src.Data.Metrics.NetElevationMeters))
            .ForMember(dest => dest.TotalElevationMeters,
                opt => opt.MapFrom(src => src.Data.Metrics.TotalElevationMeters));
    }
}

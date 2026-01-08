using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.Mapping;

public class ClimbMapping : Profile
{
    public ClimbMapping()
    {
        CreateMap<ClimbDetails, ClimbDto>()
            .ForMember(d => d.DistanceMeters, o => o.MapFrom(s => s.Metrics.DistanceMeters))
            .ForMember(d => d.Elevation, o => o.MapFrom(s => s.Metrics.NetElevationMeters))
            .ForMember(d => d.Slope, o => o.MapFrom(s => s.Metrics.Slope))
            .ForMember(d => d.MaxSlope, o => o.MapFrom(s => s.Metrics.MaxSlope));

        CreateMap<ClimbDto, ClimbDetails>()
            .ForMember(d => d.Metrics, o => o.MapFrom(src => CreateMetricsFromDto(src)));
    }

    private ClimbMetrics CreateMetricsFromDto(ClimbDto dto)
    {
        return new ClimbMetrics
        {
            DistanceMeters = dto.DistanceMeters,
            NetElevationMeters = dto.Elevation,
            Slope = dto.Slope,
            MaxSlope = dto.MaxSlope
        };
    }
}

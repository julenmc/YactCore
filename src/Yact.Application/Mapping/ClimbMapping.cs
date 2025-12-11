using AutoMapper;
using Yact.Application.Responses;
using Yact.Domain.Entities.Climb;

namespace Yact.Application.Mapping;

public class ClimbMapping : Profile
{
    public ClimbMapping()
    {
        CreateMap<ClimbData, ClimbDto>()
            .ForMember(d => d.DistanceMeters, o => o.MapFrom(s => s.Metrics.DistanceMeters))
            .ForMember(d => d.Elevation, o => o.MapFrom(s => s.Metrics.Elevation))
            .ForMember(d => d.Slope, o => o.MapFrom(s => s.Metrics.Slope))
            .ForMember(d => d.MaxSlope, o => o.MapFrom(s => s.Metrics.MaxSlope));

        CreateMap<ClimbDto, ClimbData>()
            .ForMember(d => d.Metrics, o => o.MapFrom(src => CreateMetricsFromDto(src)));
    }

    private ClimbMetrics CreateMetricsFromDto(ClimbDto dto)
    {
        return new ClimbMetrics
        {
            DistanceMeters = dto.DistanceMeters,
            Elevation = dto.Elevation,
            Slope = dto.Slope,
            MaxSlope = dto.MaxSlope
        };
    }
}

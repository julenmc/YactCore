using Yact.Application.ReadModels.Climbs;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ClimbMapper
{
    internal static ClimbBasicReadModel ToBasicModel(this ClimbReadModel climb)
    {
        return new ClimbBasicReadModel
        {
            Id = climb.Id,
            Name = climb.Name,
            DistanceMeters = climb.DistanceMeters,
            Slope = climb.Slope,
            MaxSlope = climb.MaxSlope,
            NetElevationMeters = climb.NetElevationMeters,
        };
    }

    internal static ClimbAdvancedReadModel ToAdvancedModel(this ClimbReadModel climb)
    {
        return new ClimbAdvancedReadModel
        {
            Id = climb.Id,
            Name = climb.Name,
            DistanceMeters = climb.DistanceMeters,
            Slope = climb.Slope,
            MaxSlope = climb.MaxSlope,
            NetElevationMeters = climb.NetElevationMeters,
            TotalElevationMeters = climb.TotalElevationMeters,
            // TODO: put activity climbs
        };
    }
}

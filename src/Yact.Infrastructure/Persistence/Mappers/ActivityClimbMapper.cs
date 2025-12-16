using Yact.Infrastructure.Persistence.Models.Climb;
using Entities = Yact.Domain.Entities.Climb;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityClimbMapper
{
    internal static Entities.ActivityClimb ToDomain(this ActivityClimb model)
    {
        var climbMetrics = new Entities.ClimbMetrics
        {
            DistanceMeters = model.Climb?.DistanceMeters ?? 0,
            Elevation = model.Climb?.Elevation ?? 0,
            Slope = model.Climb?.Slope ?? 0,
            MaxSlope = model.Climb?.MaxSlope ?? 0,
        };
        var data = new Entities.ClimbData
        {
            Id = model.ClimbId,
            Name = model.Climb?.Name ?? string.Empty,
            LongitudeInit = model.Climb?.LongitudeInit ?? 0,
            LongitudeEnd = model.Climb?.LongitudeEnd ?? 0,
            LatitudeInit = model.Climb?.LatitudeInit ?? 0,
            LatitudeEnd = model.Climb?.LatitudeEnd ?? 0,
            AltitudeInit = model.Climb?.AltitudeInit ?? 0,
            AltitudeEnd = model.Climb?.AltitudeEnd ?? 0,
            Metrics = climbMetrics,
            Validated = model.Climb?.Validated ?? false,
        };
        return new Entities.ActivityClimb
        {
            Id = model.Id,
            ActivityId = model.ActivityId,
            ClimbId = model.ClimbId,
            StartPointMeters = model.StartPointMeters,
            Data = data
        };
    }

    internal static ActivityClimb ToModel(this Entities.ActivityClimb entity)
    {
        return new ActivityClimb
        {
            Id = entity.Id,
            ActivityId = entity.ActivityId,
            ClimbId = entity.ClimbId,
            StartPointMeters = entity.StartPointMeters
        };
    }
}

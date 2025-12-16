using Entities = Yact.Domain.Entities.Climb;
using Yact.Infrastructure.Persistence.Models.Climb;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ClimbMapper
{
    internal static Entities.ClimbData ToDomain(this ClimbInfo model)
    {
        var metrics = new Entities.ClimbMetrics
        {
            DistanceMeters = model.DistanceMeters,
            Elevation = model.Elevation,
            Slope = model.Slope,
            MaxSlope = model.MaxSlope,
        };

        return new Entities.ClimbData
        {
            Metrics = metrics,
            Id = model.Id,
            Name = model.Name,
            LongitudeInit = model.LongitudeInit,
            LongitudeEnd = model.LongitudeEnd,
            LatitudeInit = model.LatitudeInit,
            LatitudeEnd = model.LatitudeEnd,
            AltitudeInit = model.AltitudeInit,
            AltitudeEnd = model.AltitudeEnd,
            Validated = model.Validated,
        };
    }

    internal static ClimbInfo ToModel(this Entities.ClimbData entity)
    {
        return new ClimbInfo
        {
            Id = entity.Id,
            Name = entity.Name,
            LongitudeInit = entity.LongitudeInit,
            LongitudeEnd= entity.LongitudeEnd,
            LatitudeInit = entity.LatitudeInit,
            LatitudeEnd = entity.LatitudeEnd,
            AltitudeInit = entity.AltitudeInit,
            AltitudeEnd = entity.AltitudeEnd,
            DistanceMeters = entity.Metrics.DistanceMeters,
            Slope = entity.Metrics.Slope,
            MaxSlope = entity.Metrics.MaxSlope,
            Elevation = entity.Metrics.Elevation,
            Validated = entity.Validated,
        };
    }
}

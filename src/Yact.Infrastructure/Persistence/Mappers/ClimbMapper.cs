using Yact.Domain.ValueObjects.Climb;
using Entities = Yact.Domain.Entities.Climb;
using Yact.Infrastructure.Persistence.Models;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ClimbMapper
{
    internal static Entities ToDomain(this ClimbInfo model)
    {
        var metrics = new ClimbMetrics
        {
            DistanceMeters = model.DistanceMeters,
            NetElevationMeters = model.Elevation,
            Slope = model.Slope,
            MaxSlope = model.MaxSlope,
        };

        var coordinates = new ClimbCoordinates
        {
            LatitudeInit = model.LatitudeInit,
            LatitudeEnd = model.LatitudeEnd,
            LongitudeInit = model.LongitudeInit,
            LongitudeEnd = model.LongitudeEnd,
            AltitudeInit = model.AltitudeInit,
            AltitudeEnd = model.AltitudeEnd
        };

        var details = new ClimbDetails
        {
            Metrics = metrics,
            Coordinates = coordinates,
            StartPointMeters = 0
        };

        var summary = new ClimbSummary(model.Name ?? "Unknown");

        return Entities.Load(
            ClimbId.From(model.Id),
            details,
            summary
        );
    }

    internal static ClimbInfo ToModel(this Entities entity)
    {
        return new ClimbInfo
        {
            Id = entity.Id.Value,
            Name = entity.Summary.Name,
            LongitudeInit = entity.Data.Coordinates.LongitudeInit,
            LongitudeEnd = entity.Data.Coordinates.LongitudeEnd,
            LatitudeInit = entity.Data.Coordinates.LatitudeInit,
            LatitudeEnd = entity.Data.Coordinates.LatitudeEnd,
            AltitudeInit = entity.Data.Coordinates.AltitudeInit,
            AltitudeEnd = entity.Data.Coordinates.AltitudeEnd,
            DistanceMeters = entity.Data.Metrics.DistanceMeters,
            Slope = entity.Data.Metrics.Slope,
            MaxSlope = entity.Data.Metrics.MaxSlope,
            Elevation = entity.Data.Metrics.NetElevationMeters,
            Validated = true,
        };
    }
}

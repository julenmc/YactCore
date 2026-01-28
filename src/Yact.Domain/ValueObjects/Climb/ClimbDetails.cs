namespace Yact.Domain.ValueObjects.Climb;

public record ClimbDetails
{
    public required ClimbMetrics Metrics { get; init; }
    public required ClimbCoordinates Coordinates { get; init; }
    public required double StartPointMeters { get; init; }

    public bool Match(ClimbDetails climb)
    {
        const float CoordinatesDelta = 0.0001f;
        const float MetricsDelta = 10f;

        return
            IsClose(Coordinates.LatitudeInit, climb.Coordinates.LatitudeInit, CoordinatesDelta) &&
            IsClose(Coordinates.LatitudeEnd, climb.Coordinates.LatitudeEnd, CoordinatesDelta) &&
            IsClose(Coordinates.LongitudeInit, climb.Coordinates.LongitudeInit, CoordinatesDelta) &&
            IsClose(Coordinates.LongitudeEnd, climb.Coordinates.LongitudeEnd, CoordinatesDelta) &&
            IsClose(Coordinates.AltitudeInit, climb.Coordinates.AltitudeInit, MetricsDelta) &&
            IsClose(Coordinates.AltitudeEnd, climb.Coordinates.AltitudeEnd, MetricsDelta) &&
            IsClose(Metrics.DistanceMeters, climb.Metrics.DistanceMeters, MetricsDelta) &&
            IsClose(Metrics.NetElevationMeters, climb.Metrics.NetElevationMeters, MetricsDelta);
    }

    private static bool IsClose(double a, double b, double delta)
    {
        var abs = Math.Abs(a - b);
        return abs <= delta;
    }
}

namespace Yact.Domain.Entities.Climb;

public class ClimbData
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double LongitudeInit { get; set; }
    public double LongitudeEnd { get; set; }
    public double LatitudeInit { get; set; }
    public double LatitudeEnd { get; set; }
    public double AltitudeInit { get; set; }
    public double AltitudeEnd { get; set; }
    public required ClimbMetrics Metrics { get; set; }
    public bool Validated { get; set; }

    public bool Match(ClimbData climb)
    {
        const float CoordinatesDelta = 0.0001f;
        const float MetricsDelta = 10f;

        return
            IsClose(LatitudeInit, climb.LatitudeInit, CoordinatesDelta) &&
            IsClose(LatitudeEnd, climb.LatitudeEnd, CoordinatesDelta) &&
            IsClose(LongitudeInit, climb.LongitudeInit, CoordinatesDelta) &&
            IsClose(LongitudeEnd, climb.LongitudeEnd, CoordinatesDelta) &&
            IsClose(AltitudeInit, climb.AltitudeInit, MetricsDelta) &&
            IsClose(AltitudeEnd, climb.AltitudeEnd, MetricsDelta) &&
            IsClose(Metrics.DistanceMeters, climb.Metrics.DistanceMeters, MetricsDelta) &&
            IsClose(Metrics.Elevation, climb.Metrics.Elevation, MetricsDelta);
    }

    private static bool IsClose(double a, double b, double delta)
    {
        var abs = Math.Abs(a - b);
        return abs <= delta;
    }
}

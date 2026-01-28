namespace Yact.Domain.ValueObjects.Climb;

public record ClimbCoordinates
{
    public double LongitudeInit { get; init; }
    public double LongitudeEnd { get; init; }
    public double LatitudeInit { get; init; }
    public double LatitudeEnd { get; init; }
    public double AltitudeInit { get; init; }
    public double AltitudeEnd { get; init; }
}

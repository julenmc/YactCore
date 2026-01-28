
namespace Yact.Domain.ValueObjects.Climb;

public record ClimbMetrics
{
    public double DistanceMeters { get; init; }
    public double Slope { get; init; }
    public double MaxSlope { get; init; }
    public double NetElevationMeters { get; init; }
    public double TotalElevationMeters { get; init; }
}

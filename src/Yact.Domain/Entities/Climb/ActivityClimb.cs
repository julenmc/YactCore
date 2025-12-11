namespace Yact.Domain.Entities.Climb;

public class ActivityClimb
{
    public required ClimbData Data { get; set; }
    public required double StartPointMeters { get; set; }
    public double EndPointMeters => StartPointMeters + Data.Metrics.DistanceMeters;
}
using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Entities.Climb;

public class ActivityClimb
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public int ClimbId { get; set; }
    public int IntervalId { get; set; }
    public required ClimbData Data { get; set; }
    public ActivityInfo? Activity { get; set; }
    public required double StartPointMeters { get; set; }
    public double EndPointMeters => StartPointMeters + Data.Metrics.DistanceMeters;
}
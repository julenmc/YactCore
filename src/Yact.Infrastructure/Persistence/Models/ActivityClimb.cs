namespace Yact.Infrastructure.Persistence.Models;

public class ActivityClimb
{
    public Guid Id { get; set; }
    public double StartPointMeters { get; set; }

    // Foreign Key
    public required Guid ActivityId { get; set; }
    public ActivityInfo? Activity { get; set; }
    public required Guid ClimbId { get; set; }
    public ClimbInfo? Climb { get; set; }
}

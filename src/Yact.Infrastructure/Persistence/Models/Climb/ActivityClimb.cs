using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Models.Analytics;

namespace Yact.Infrastructure.Persistence.Models.Climb;

public class ActivityClimb
{
    public int Id { get; set; }
    public double StartPointMeters { get; set; }

    // Foreign Key
    public required int ActivityId { get; set; }
    public ActivityInfo? Activity { get; set; }
    public required int ClimbId { get; set; }
    public ClimbInfo? Climb { get; set; }
}

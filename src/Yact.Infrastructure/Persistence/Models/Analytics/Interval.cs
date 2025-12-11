using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Models.Climb;

namespace Yact.Infrastructure.Persistence.Models.Analytics;

public class Interval
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public float Distance { get; set; }
    public float AverageHeartRate { get; set; }
    public float AveragePower { get; set; }
    public float AverageCadence { get; set; }
    public float MaxHeartRate { get; set; }
    public float MaxPower { get; set; }
    public float MaxCadence { get; set; }

    // Foreign Key
    public required int ActivityInfoId { get; set; }
    public ActivityInfo? ActivityInfo { get; set; }
}

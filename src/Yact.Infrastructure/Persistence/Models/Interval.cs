namespace Yact.Infrastructure.Persistence.Models;

public class Interval
{
    public Guid Id { get; set; }
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
    public required Guid ActivityInfoId { get; set; }
    public ActivityInfo? ActivityInfo { get; set; }
}

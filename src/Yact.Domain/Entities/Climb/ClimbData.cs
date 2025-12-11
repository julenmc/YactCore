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
}

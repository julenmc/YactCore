namespace Yact.Infrastructure.Persistence.Models.Climb;

public class ClimbInfo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double LongitudeInit { get; set; }
    public double LongitudeEnd { get; set; }
    public double LatitudeInit { get; set; }
    public double LatitudeEnd { get; set; }
    public double AltitudeInit { get; set; }
    public double AltitudeEnd { get; set; }
    public double DistanceMeters { get; set; }
    public double Slope { get; set; }
    public double MaxSlope { get; set; }
    public double Elevation { get; set; }
    public bool Validated { get; set; }

    // 1-N relation with efforts
    public List<ActivityClimb>? Efforts { get; set; }
}

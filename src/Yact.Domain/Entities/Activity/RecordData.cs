namespace Yact.Domain.Entities.Activity;

public class RecordData
{
    public DateTime Timestamp { get; set; }
    public CoordinatesData Coordinates { get; set; } = new();
    public float? DistanceMeters { get; set; }
    public float? Slope { get; set; }
    public float? SpeedMps { get; set; }
    public int? HeartRate { get; set; }
    public float? Power { get; set; }
    public int? Cadence { get; set; }
}

public class CoordinatesData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Altitude { get; set; }
}

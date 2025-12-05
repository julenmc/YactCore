namespace Yact.Domain.Entities.Activity;

public class RecordData
{
    public DateTime Timestamp { get; set; }
    public CoordinatesData? Coordinates { get; set; }
    public double? SpeedMps { get; set; }
    public int? HeartRate { get; set; }
    public double? Power { get; set; }
    public int? Cadence { get; set; }
}

public class CoordinatesData
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Altitude { get; set; }
}

namespace Yact.Application.Responses;

public class ClimbResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    //public double LongitudeInit { get; set; }
    //public double LongitudeEnd { get; set; }
    //public double LatitudeInit { get; set; }
    //public double LatitudeEnd { get; set; }
    //public double AltitudeInit { get; set; }
    //public double AltitudeEnd { get; set; }
    public double DistanceMeters { get; set; }
    public double Slope { get; set; }
    public double MaxSlope { get; set; }
    public double NetElevationMeters { get; set; }
    public double TotalElevationMeters { get; set; }
}

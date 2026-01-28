namespace Yact.Application.ReadModels.Climbs;

public record ClimbAdvancedReadModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double DistanceMeters { get; set; }
    public double Slope { get; set; }
    public double MaxSlope { get; set; }
    public double NetElevationMeters { get; set; }
    public double TotalElevationMeters { get; set; }
    // Activity climbs list
}

namespace Yact.Application.ReadModels.Activities;

public record ActivityBasicReadModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public double DistanceMeters { get; set; }
    public double ElevationMeters { get; set; }
    public string? Type { get; set; }
}

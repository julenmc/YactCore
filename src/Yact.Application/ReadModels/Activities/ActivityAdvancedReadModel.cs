using Yact.Application.ReadModels.ActivityClimbs;

namespace Yact.Application.ReadModels.Activities;

public record ActivityAdvancedReadModel
{
    public Guid Id { get; set; }
    public Guid CyclistId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Path { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DistanceMeters { get; set; }
    public double ElevationMeters { get; set; }
    public string? Type { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    // Activity climbs
    public IEnumerable<ActivityClimbFromActivityReadModel> ActivityClimbs { get; set; }
        = Enumerable.Empty<ActivityClimbFromActivityReadModel>();
    // Intervals
}

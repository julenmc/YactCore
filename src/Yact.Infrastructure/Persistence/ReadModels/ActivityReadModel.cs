namespace Yact.Infrastructure.Persistence.ReadModels;

public class ActivityReadModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Path { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DistanceMeters { get; set; }
    public double ElevationMeters { get; set; }
    public string? Type { get; set; }
    public DateTime? CreationDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    // Foreign Key
    public Guid CyclistId { get; set; }
    public CyclistReadModel? Cyclist { get; set; }

    // 1-N relation for climbs and intervals
    public IEnumerable<ActivityClimbReadModel> ActivityClimbs { get; set; }
        = new List<ActivityClimbReadModel>();
}

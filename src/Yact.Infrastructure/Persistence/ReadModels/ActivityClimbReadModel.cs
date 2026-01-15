namespace Yact.Infrastructure.Persistence.ReadModels;

public record ActivityClimbReadModel
{
    public Guid Id { get; set; }
    public double StartPointMeters { get; set; }

    // Foreign Keys
    public required Guid ActivityId { get; set; }
    public ActivityReadModel? Activity { get; set; }
    public required Guid ClimbId { get; set; }
    public ClimbReadModel? Climb { get; set; }
}

namespace Yact.Application.ReadModels.ActivityClimbs;

public record ActivityClimbFromActivityReadModel
{
    public Guid Id { get; set; }
    public double StartPointMeters { get; set; }
    public string ClimbName { get; set; } = string.Empty;
}

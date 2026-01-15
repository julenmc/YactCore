namespace Yact.Infrastructure.Persistence.ReadModels;

public class CyclistReadModel
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    // 1-N relations
    public required IEnumerable<CyclistFitnessReadModel> Fitnesses { get; set; }
    public IEnumerable<ActivityReadModel> Activities { get; set; } 
        = Enumerable.Empty<ActivityReadModel>();
}

namespace Yact.Infrastructure.Persistence.Models;

public class Cyclist
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    // 1-N relations
    public List<CyclistFitness>? Fitnesss { get; set; }
    public List<ActivityInfo>? Activities { get; set; }
}

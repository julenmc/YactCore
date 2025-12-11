using Yact.Infrastructure.Persistence.Models.Activity;

namespace Yact.Infrastructure.Persistence.Models.Cyclist;

public class CyclistInfo
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    // 1-N relations
    public List<CyclistFitness>? Fitnesss { get; set; }
    public List<ActivityInfo>? Activities { get; set; }
}

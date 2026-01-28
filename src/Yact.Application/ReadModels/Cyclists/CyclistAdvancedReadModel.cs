namespace Yact.Application.ReadModels.Cyclists;

public record CyclistAdvancedReadModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";
    public DateTime BirthDate { get; set; }
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    public required IEnumerable<CyclistFitnessReadModel>? FitnessHistory { get; set; }
    public CyclistFitnessReadModel? LatestFitness =>
        FitnessHistory?.OrderByDescending(f => f.UpdateDate).First();
    public required IEnumerable<Activities.ActivityBasicReadModel>? Activities { get; set; }
}

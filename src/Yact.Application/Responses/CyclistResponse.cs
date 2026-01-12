namespace Yact.Application.Responses;

public class CyclistResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";
    public DateTime BirthDate { get; set; }
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    public required List<CyclistFitnessResponse> FitnessHistory { get; set; }
    public CyclistFitnessResponse LatestFitness =>
        FitnessHistory.OrderByDescending(f => f.UpdateDate).First();
}

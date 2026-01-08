namespace Yact.Application.Responses;

public class CyclistDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? FullName => $"{Name} {LastName}";
    public DateTime BirthDate { get; set; }
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    public CyclistFitnessDto? FitnessData { get; set; }
}

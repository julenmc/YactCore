namespace Yact.Application.ReadModels.Cyclists;

public record CyclistBasicReadModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string FullName => $"{Name} {LastName}";
    public DateTime BirthDate { get; set; }
    public int Age => DateTime.UtcNow.Year - BirthDate.Year - (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
}

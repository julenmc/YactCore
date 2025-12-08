namespace Yact.Infrastructure.Persistence.Models.Cyclist;

public class CyclistInfo
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}

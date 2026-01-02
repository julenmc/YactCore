namespace Yact.Application.Responses;

public class ActivityDto
{
    public int Id { get; set; }
    public int CyclistId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Path { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DistanceMeters { get; set; }
    public double ElevationMeters { get; set; }
    public string? Type { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}

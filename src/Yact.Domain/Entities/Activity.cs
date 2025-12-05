using System.ComponentModel.DataAnnotations;

namespace Yact.Domain.Models;

public class Activity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string? Path { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public double DistanceMeters { get; set; }
    public double ElevationMeters { get; set; }
    public bool IsIndoor { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}

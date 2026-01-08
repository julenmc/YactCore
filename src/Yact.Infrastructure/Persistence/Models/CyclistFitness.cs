namespace Yact.Infrastructure.Persistence.Models;

public class CyclistFitness
{
    public required Guid Id { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public string? PowerCurveJson { get; set; }
    public string? HrZonesRaw { get; set; }
    public string? PowerZonesRaw { get; set; }

    // Foreing Key
    public required Guid CyclistId { get; set; }
    public CyclistInfo? Cyclist { get; set; }
}

namespace Yact.Application.Responses;

public class CyclistFitnessDto
{
    public Guid Id { get; set; }
    public Guid CyclistId { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public PowerCurveDto? PowerCurve { get; set; }
    public Dictionary<string, ZoneDto>? HrZones { get; set; }
    public Dictionary<string, ZoneDto>? PowerZones { get; set; }
}

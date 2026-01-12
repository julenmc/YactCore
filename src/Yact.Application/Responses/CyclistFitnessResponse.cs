using Yact.Application.Common;

namespace Yact.Application.Responses;

public class CyclistFitnessResponse
{
    public Guid Id { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public PowerCurveResponse? PowerCurve { get; set; }
    public Dictionary<string, Zone>? HrZones { get; set; }
    public Dictionary<string, Zone>? PowerZones { get; set; }
}

using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities.Cyclist;

public class CyclistFitness
{
    public int Id { get; set; }
    public int CyclistId { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public PowerCurve? PowerCurve { get; set; }
    public Dictionary<int, Zone>? HrZones { get; set; }
    public Dictionary<int, Zone>? PowerZones { get; set; }
}

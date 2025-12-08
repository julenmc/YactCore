namespace Yact.Domain.Entities.Cyclist;

public class CyclistFitness
{
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public PowerCurve? PowerCurve { get; set; }
    public Zones? HrZones { get; set; }
    public Zones? PowerZones { get; set; }
}

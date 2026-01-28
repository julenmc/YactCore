using Yact.Application.ReadModels.Common;

namespace Yact.Application.ReadModels.Cyclists;

public record CyclistFitnessReadModel
{
    public Guid Id { get; set; }
    public DateTime UpdateDate { get; set; }
    public ushort Height { get; set; }
    public float Weight { get; set; }
    public ushort Ftp { get; set; }
    public float Vo2Max { get; set; }
    public IDictionary<int, int>? PowerCurve { get; set; }
    public IDictionary<string, ZoneReadModel>? HrZones { get; set; }
    public IDictionary<string, ZoneReadModel>? PowerZones { get; set; }
}

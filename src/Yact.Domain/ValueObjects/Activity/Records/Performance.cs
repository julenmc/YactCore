namespace Yact.Domain.ValueObjects.Activity.Records;

public record Performance
{
    public int? HeartRate { get; init; }
    public float? Power { get; init; }
    public int? Cadence { get; init; }
    public float? SpeedMps { get; init; }
};

namespace Yact.Domain.ValueObjects.Activity.Intervals;

internal record Thresholds
{
    public float CvStart { get; init; }
    public float CvFollow { get; init; }
    public float Range { get; init; }
    public float MaRel { get; init; }
}

internal record IntervalThresholdValues
{
    public Thresholds Max { get; init; } = new();
    public Thresholds Min { get; init; } = new();
    public Thresholds Default { get; init; } = new();
}

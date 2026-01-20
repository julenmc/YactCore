namespace Yact.Domain.ValueObjects.Activity.Intervals;

public record Thresholds
{
    public float CvStart { get; init; }
    public float CvFollow { get; init; }
    public float Range { get; init; }
    public float MaRel { get; init; }
}

public record IntervalThresholdValues
{
    public Thresholds Max { get; init; } = new();
    public Thresholds Min { get; init; } = new();
    public Thresholds Default { get; init; } = new();
}

public record IntervalGroupThresholds
{
    public Thresholds Short { get; init; } = new();
    public Thresholds Medium { get; init; } = new();
    public Thresholds Long { get; init; } = new();
}

using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

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

    public bool IsAllowed(Thresholds thresholds)
    {
        return thresholds.CvStart >= Min.CvStart && thresholds.CvStart <= Max.CvStart &&
            thresholds.CvFollow >= Min.CvFollow && thresholds.CvFollow <= Max.CvFollow &&
            thresholds.Range >= Min.Range && thresholds.Range <= Max.Range &&
            thresholds.MaRel >= Min.MaRel && thresholds.MaRel <= Max.MaRel;
    }
}

public record IntervalGroupThresholds
{
    public Thresholds Short { get; private set; }
    public Thresholds Medium { get; private set; }
    public Thresholds Long { get; private set; }

    private IntervalGroupThresholds(
        Thresholds shortThr,
        Thresholds mediumThr,
        Thresholds longThr)
    {
        Short = shortThr;
        Medium = mediumThr;
        Long = longThr;
    }

    public static IntervalGroupThresholds Create (
        Thresholds shortThr,
        Thresholds mediumThr,
        Thresholds longThr)
    {
        // Check if thresholds are inside allowed limits
        if (!IntervalSearchValues.ShortIntervals.IsAllowed(shortThr) ||
            !IntervalSearchValues.MediumIntervals.IsAllowed(mediumThr) ||
            !IntervalSearchValues.LongIntervals.IsAllowed(longThr))
            throw new ArgumentException("Invalid thresholds. At least one of them is out of the limits.");

        return new IntervalGroupThresholds(shortThr, mediumThr, longThr);
    }
}

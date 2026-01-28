using Yact.Domain.ValueObjects.Activity.Intervals;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal static class IntervalSearchValues
{
    public readonly static IntervalThresholdValues ShortIntervals = new IntervalThresholdValues
    {
        Max     = new Thresholds{CvStart = 0.20f, CvFollow = 0.30f, Range = 0.30f, MaRel = 0.20f},
        Min     = new Thresholds{CvStart = 0.10f, CvFollow = 0.10f, Range = 0.10f, MaRel = 0.10f},
        Default = new Thresholds{CvStart = 0.15f, CvFollow = 0.15f, Range = 0.20f, MaRel = 0.10f}
    };
    public readonly static IntervalThresholdValues MediumIntervals = new IntervalThresholdValues
    {
        Max     = new Thresholds{CvStart = 0.40f, CvFollow = 0.50f, Range = 0.70f, MaRel = 0.30f},
        Min     = new Thresholds{CvStart = 0.20f, CvFollow = 0.10f, Range = 0.30f, MaRel = 0.10f},
        Default = new Thresholds{CvStart = 0.20f, CvFollow = 0.25f, Range = 0.50f, MaRel = 0.12f}
    };

    public readonly static IntervalThresholdValues LongIntervals = new IntervalThresholdValues
    {
        Max     = new Thresholds{CvStart = 0.60f, CvFollow = 0.60f, Range = 2.00f, MaRel = 0.50f},
        Min     = new Thresholds{CvStart = 0.20f, CvFollow = 0.20f, Range = 0.50f, MaRel = 0.10f},
        Default = new Thresholds{CvStart = 0.25f, CvFollow = 0.30f, Range = 0.75f, MaRel = 0.15f}
    };
}

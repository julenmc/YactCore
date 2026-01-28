namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal static class IntervalTimes
{
    public const ushort IntervalMinTime = 30;      // 30 seconds
    public const ushort SprintMinTime = 5;
    internal const ushort MediumIntervalMinTime = 240;      // Less than 4 minutes is considered a short interval
    internal const ushort LongIntervalMinTime = 600;        // Less than 10 minutes is considered a medium interval
    internal const int ShortWindowSize = 10;
    internal const int MediumWindowSize = 30;
    internal const int LongWindowSize = 60;
}

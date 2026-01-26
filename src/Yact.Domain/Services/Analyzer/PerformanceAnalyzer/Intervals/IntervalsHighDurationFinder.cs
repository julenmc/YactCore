using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsHighDurationFinder : IntervalsFinder
{
    private const int WindowSizeHighDuration = 30;
    private const float CvAllowed = 0.2f;
    private const float DeltaMax = 0.15f;
    private const float DeviationAllowed = 0.2f;

    private readonly int _minPower;

    internal IntervalsHighDurationFinder(
        PowerZones powerZones,
        IEnumerable<RecordData> records)
        : base(records, powerZones, WindowSizeHighDuration, CvAllowed, DeviationAllowed, DeltaMax)
    {
        _minPower = (powerZones.Values[2].HighLimit + powerZones.Values[2].LowLimit) / 2;
    }

    protected override bool IsConsideredAnInterval(IntervalSummary interval)
    {
        return false;
    }
}

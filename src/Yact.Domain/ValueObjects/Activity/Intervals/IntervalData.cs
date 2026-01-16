using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.ValueObjects.Activity.Intervals;

public record IntervalData
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public int DurationSeconds => (int)(EndTime - StartTime).TotalSeconds + 1;
    public double TotalDistance { get; init; }
    public int AverageHeartRate { get; init; }
    public int AveragePower { get; init; }
    public int AverageCadence { get; init; }

    public bool IsConsideredAnInterval(IDictionary<int, Zone> zones)
    {
        var shortZone = zones[IntervalZones.IntervalMinZones[IntervalGroups.Short]];
        if (shortZone == null)
            throw new ArgumentException($"Couldn't find minimum zone for '{IntervalGroups.Short}' (Id = {IntervalZones.IntervalMinZones[IntervalGroups.Short]}).");

        var mediumZone = zones[IntervalZones.IntervalMinZones[IntervalGroups.Medium]];
        if (mediumZone == null)
            throw new ArgumentException($"Couldn't find minimum zone for '{IntervalGroups.Medium}' (Id = {IntervalZones.IntervalMinZones[IntervalGroups.Medium]}).");

        var longZone = zones[IntervalZones.IntervalMinZones[IntervalGroups.Long]];
        if (longZone == null)
            throw new ArgumentException($"Couldn't find minimum zone for '{IntervalGroups.Long}' (Id = {IntervalZones.IntervalMinZones[IntervalGroups.Long]}).");

        double shortLimit = shortZone.LowLimit;
        double mediumLimit = mediumZone.LowLimit;
        double longLimit = longZone.LowLimit;

        return DurationSeconds switch
        {
            >= IntervalTimes.LongIntervalMinTime => AveragePower >= longLimit,
            >= IntervalTimes.MediumIntervalMinTime => AveragePower >= mediumLimit,
            >= IntervalTimes.IntervalMinTime => AveragePower >= shortLimit,
            _ => false
        };
    }
}

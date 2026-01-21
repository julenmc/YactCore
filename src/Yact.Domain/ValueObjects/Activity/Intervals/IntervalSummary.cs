using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.ValueObjects.Activity.Intervals;

public record IntervalSummary
{
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public int DurationSeconds => (int)(EndTime - StartTime).TotalSeconds + 1;
    public double TotalDistance { get; private set; }
    public int AverageHeartRate { get; private set; }
    public int AveragePower { get; private set; }
    public int AverageCadence { get; private set; }

    public static IntervalSummary Create(
        DateTime startTime,
        DateTime endTime,
        IEnumerable<RecordData> records)
    {
        var points = records
            .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
            .ToList();

        return new IntervalSummary
        {
            StartTime = startTime,
            EndTime = endTime,
            TotalDistance = points.Last().DistanceMeters - points.First().DistanceMeters,
            AveragePower = (int)points.Average(p => p.Performance?.Power ?? 0),
            AverageHeartRate = (int)points.Average(p => p.Performance?.HeartRate ?? 0),
            AverageCadence = (int)points.Average(p => p.Performance?.Cadence ?? 0),
        };
    }

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

    public Collision CheckCollisionWithOtherInterval(IntervalSummary interval)
    {
        // Check if don't collide
        if (StartTime > interval.EndTime ||
            EndTime < interval.StartTime)
            return Collision.None;

        // They collide
        return StartTime.CompareTo(interval.StartTime) switch
        {
            0 => EndTime.CompareTo(interval.EndTime) switch
            {
                0 => Collision.Same,
                < 0 => Collision.IsParent,
                > 0 => Collision.IsChild
            },
            < 0 => EndTime.CompareTo(interval.EndTime) switch
            {
                < 0 => Collision.After,
                _ => Collision.IsChild
            },
            > 0 => EndTime.CompareTo(interval.EndTime) switch
            {
                > 0 => Collision.Before,
                _ => Collision.IsParent
            }
        };
    }

    public enum Collision
    {
        None, 
        Before,
        After,
        IsChild,
        IsParent,
        Same
    }
}

using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Primitives;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class Interval : Entity<IntervalId>
{
    public IntervalSummary Summary { get; private set; }
    public ICollection<RecordData> Records => _records;
    public ICollection<Interval> SubIntervals => _subIntervals;

    private readonly List<RecordData> _records = new();
    private readonly List<Interval> _subIntervals = new();

    private Interval(
        IntervalId id,
        IntervalSummary summary,
        IEnumerable<RecordData>? records = null) : base(id)
    {
        Summary = summary;
        if (records != null)
            _records = records.ToList();
    }

    internal static IEnumerable<Interval> Create(
        IDictionary<int, Zone> powerZones,
        IEnumerable<RecordData> records,
        IntervalGroupThresholds thresholds,
        EventHandler<string>? logEventHandler = null)
    {
        if (records.Count() == 0)
            return Enumerable.Empty<Interval>();

        // Finder
        var shortFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Short,
            thresholds: thresholds.Short);
        shortFinder.LogEventHandler += logEventHandler;
        logEventHandler?.Invoke(typeof(Interval), "Short interval searching starts.");
        var foundIntervals = shortFinder.Search(records).ToList();

        var mediumFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Medium,
            foundIntervals,
            thresholds.Medium);
        mediumFinder.LogEventHandler += logEventHandler;
        logEventHandler?.Invoke(typeof(Interval), "Medium interval searching starts.");
        foundIntervals.AddRange(mediumFinder.Search(records));

        var longFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Long,
            foundIntervals,
            thresholds.Long);
        longFinder.LogEventHandler += logEventHandler;
        logEventHandler?.Invoke(typeof(Interval), "Long interval searching starts.");
        foundIntervals.AddRange(longFinder.Search(records));

        // Create merges
        var merger = new IntervalsMerger(records);
        merger.CreateAndInsertMerged(foundIntervals);
        logEventHandler?.Invoke(typeof(Interval), $"Intervals merged");

        // Create interval entities and integrate sub-intervals
        var result = new List<Interval>();
        var sortedByDurationList = foundIntervals.OrderByDescending(i => i.DurationSeconds).ToList();
        for (int i = 0; i < sortedByDurationList.Count; i++)
        {
            // Create entity
            var entity = Create(IntervalId.NewId(), sortedByDurationList[i], records);
            result.Add(entity);

            for (int j = 0; j < foundIntervals.Count; j++)
            {
                if (entity.IsSubInterval(foundIntervals[j]))
                {
                    // Add sub-interval
                    entity.AddSubInterval(foundIntervals[j], records);

                    // Remove sub-interval from lists
                    sortedByDurationList.Remove(foundIntervals[j]);
                    foundIntervals.RemoveAt(j);
                }
            }
        }

        // Handle collisions
        var collisionService = new IntervalsCollisionsService();
        collisionService.LogEventHandler += logEventHandler;
        collisionService.RemoveCollisions(result);
        logEventHandler?.Invoke(typeof(Interval), $"Collisions handled, {result.Count} intervals after collision service");

        return result;
    }

    internal static Interval Create(
        IntervalId id,
        IntervalSummary summary,
        IEnumerable<RecordData> records)
    {
        var intervalRecords = records
            .Where(i => i.Timestamp >= summary.StartTime && i.Timestamp <= summary.EndTime)
            .ToList();
        return new Interval(id, summary, intervalRecords);
    }

    internal static Interval Load(
        IntervalId id,
        IntervalSummary summary)
    {
        return new Interval(id, summary);
    }

    internal bool IsSubInterval(IntervalSummary data)
    {
        return Summary.CheckCollisionWithOtherInterval(data) == IntervalSummary.Collision.IsChild;
    }

    internal void Trim(
        DateTime startTime,
        DateTime endTime)
    {
        if (_records.Count == 0)
            throw new NoDataException("Interval has no records saved");

        var newStart = Summary.StartTime.CompareTo(startTime) < 0 ? startTime : Summary.StartTime;
        var newEnd = Summary.EndTime.CompareTo(endTime) > 0 ? endTime : Summary.EndTime;

        Summary = IntervalSummary.Create(newStart, newEnd, _records);
        var trimRecords = _records
            .Where(i => i.Timestamp >= Summary.StartTime && i.Timestamp <= Summary.EndTime)
            .ToList();

        // Trim sub-intervals
        foreach(var interval in _subIntervals)
        {
            interval.Trim(newStart, newEnd);
        }
    }

    internal void AddSubInterval(
        IntervalSummary summary,
        IEnumerable<RecordData> records)
    {
        var subInterval = Create(IntervalId.NewId(), summary, records);
        _subIntervals.Add(subInterval);
    }
}

using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.Primitives;
using Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Entities;

public class Interval : Entity<IntervalId>
{
    public IntervalSummary Summary { get; }
    public IEnumerable<RecordData>? Records { get; }
    public ICollection<Interval> SubIntervals => _subIntervals;

    private readonly List<Interval> _subIntervals = new();

    private Interval(
        IntervalId id,
        IntervalSummary summary,
        IEnumerable<RecordData>? records = null) : base(id)
    {
        Summary = summary;
        Records = records;
    }

    internal static IEnumerable<Interval> Create(
        IDictionary<int, Zone> powerZones,
        IEnumerable<RecordData> records,
        IntervalGroupThresholds thresholds)
    {
        // Finder
        var shortFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Short,
            thresholds: thresholds.Short);
        var foundIntervals = shortFinder.Search(records);

        var mediumFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Medium,
            foundIntervals,
            thresholds.Medium);
        foundIntervals = mediumFinder.Search(records);

        var longFinder = new IntervalsFinder(
            powerZones,
            IntervalSearchGroups.Long,
            foundIntervals,
            thresholds.Long);
        foundIntervals = longFinder.Search(records);

        // Create merges
        var intervalsList = foundIntervals.ToList();
        var merger = new IntervalsMerger(records);
        merger.CreateAndInsertMerged(intervalsList);

        // Create interval entities and integrate sub-intervals
        var result = new List<Interval>();
        var sortedByDurationList = intervalsList.OrderByDescending(i => i.DurationSeconds).ToList();
        for (int i = 0; i < sortedByDurationList.Count; i++)
        {
            // Create entity
            var entity = Create(IntervalId.NewId(), sortedByDurationList[i], records);
            result.Add(entity);

            for (int j = 0; j < intervalsList.Count; j++)
            {
                if (entity.IsSubInterval(intervalsList[j]))
                {
                    // Add sub-interval
                    entity.AddSubInterval(intervalsList[j], records);

                    // Remove sub-interval from lists
                    intervalsList.RemoveAt(j);
                    sortedByDurationList.Remove(intervalsList[j]);
                }
            }
        }

        // Handle collisions

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

    private void AddSubInterval(
        IntervalSummary summary,
        IEnumerable<RecordData> records)
    {
        var subInterval = Create(IntervalId.NewId(), summary, records);
        _subIntervals.Add(subInterval);
    }
}

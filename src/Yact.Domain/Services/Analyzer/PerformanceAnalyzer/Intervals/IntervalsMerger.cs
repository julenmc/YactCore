using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsMerger
{
    private readonly List<string> _debugTrace = new();
    private readonly IEnumerable<RecordData> _records;

    internal IntervalsMerger(IEnumerable<RecordData> records)
    {
        _records = records;
    }

    internal void CreateAndInsertMerged(List<IntervalData> intervals)
    {
        _debugTrace.Clear();

        // Sort by date to make comparisons faster
        intervals.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        for (int i = 0; i < intervals.Count; i++)
        {
            int j = i + 1;
            while (j < intervals.Count)
            {
                var collision = intervals[i].CheckCollisionWithOtherInterval(intervals[j]);
                if (collision == IntervalData.Collision.Before || collision == IntervalData.Collision.After)
                {
                    var merged = MergeIntervals(intervals[i], intervals[j]);
                    if (merged != null)
                    {
                        intervals.Insert(j, merged);
                        j++;    // So it doesn't check with the generated one in the next iteration
                    }
                }
                else if (collision == IntervalData.Collision.None)
                {
                    // No collision, intervals are sorted by date, so we can move to the next interval
                    break;
                }
                j++;
            }
        }
    }

    private IntervalData? MergeIntervals(IntervalData interval1, IntervalData interval2)
    {
        IntervalData firstInterval;
        IntervalData secondInterval;
        if (interval1.StartTime < interval2.StartTime)
        {
            firstInterval = interval1;
            secondInterval = interval2;
        }
        else
        {
            firstInterval = interval2;
            secondInterval = interval1;
        }

        // Initial check if can be merged
        float maRelThr = firstInterval.DurationSeconds switch
        {
            >= IntervalTimes.LongIntervalMinTime => IntervalSearchValues.LongIntervals.Default.MaRel,
            >= IntervalTimes.MediumIntervalMinTime => IntervalSearchValues.MediumIntervals.Default.MaRel,
            _ => IntervalSearchValues.ShortIntervals.Default.MaRel
        };
        float allowedDeviation = Math.Max(firstInterval.AveragePower, secondInterval.AveragePower) * maRelThr;
        if (Math.Abs(firstInterval.AveragePower - secondInterval.AveragePower) > allowedDeviation)
        {
            return null;
            //throw new MergeExpection($"Average deviation is too high: {interval2.AveragePower} vs {interval1.AveragePower}, where allowed is {allowedDeviation}");
        }

        // Once checked proceed with merge
        DateTime startTime = firstInterval.StartTime;
        DateTime endTime = secondInterval.EndTime;
        var merged = IntervalData.Create(startTime, endTime, _records);
        _debugTrace.Add($"Interval has been extended to: {merged.StartTime.TimeOfDay}-{merged.EndTime.TimeOfDay} ({merged.DurationSeconds} s) at {merged.AveragePower} W");

        return merged;
    }
}

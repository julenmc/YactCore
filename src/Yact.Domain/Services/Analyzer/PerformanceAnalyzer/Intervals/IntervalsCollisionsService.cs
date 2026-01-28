using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Activity.Intervals;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsCollisionsService
{
    public event EventHandler<string>? LogEventHandler;

    internal void RemoveCollisions(List<Interval> intervals)
    {
        intervals.Sort((a, b) => a.Summary.StartTime.CompareTo(b.Summary.StartTime));

        for (int i = 0; i < intervals.Count; i++)
        {
            int j = i + 1;
            while (j < intervals.Count)
            {
                // The interval with highest average power will have priority, the other one will be trimmed
                var (low, high) = intervals[i].Summary.AveragePower > intervals[j].Summary.AveragePower
                    ? (intervals[j], intervals[i])
                    : (intervals[i], intervals[j]);

                var collision = high.Summary.CheckCollisionWithOtherInterval(low.Summary);
                if (collision == IntervalSummary.Collision.Before)    
                {
                    LogEventHandler?.Invoke(this, 
                        $"Collision at {high.Summary.StartTime.TimeOfDay}");

                    low.Trim(low.Summary.StartTime, high.Summary.StartTime);
                }
                else if (collision == IntervalSummary.Collision.After)
                {
                    LogEventHandler?.Invoke(this,
                        $"Collision at {high.Summary.EndTime.TimeOfDay}");

                    low.Trim(high.Summary.EndTime, low.Summary.EndTime);
                }
                else if (collision == IntervalSummary.Collision.None)
                {
                    // No collision, intervals are sorted by date, so we can move to the next interval
                    break;
                }
                j++;
            }
        }
    }
}

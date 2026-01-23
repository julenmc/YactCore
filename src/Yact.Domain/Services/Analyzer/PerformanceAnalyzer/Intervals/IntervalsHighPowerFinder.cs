using System.Drawing;
using System.Linq;
using Yact.Domain.Services.Utils.Smoothers.Metrics;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;
using static Yact.Domain.Services.Utils.Smoothers.Metrics.MovingAveragesMetricsService;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsHighPowerFinder
{
    private const int WindowSize = 10;

    private readonly PowerZones _powerZones;

    public event EventHandler<string>? LogEventHandler;

    internal IntervalsHighPowerFinder(PowerZones powerZones)
    {
        _powerZones = powerZones;
    }

    internal IEnumerable<IntervalSummary> Search(IEnumerable<RecordData> records)
    {
        throw new NotImplementedException();
    }

    internal IEnumerable<IntervalSummary> SearchHighPower(IEnumerable<RecordData> records)
    {
        // Check input
        if (records.Count() == 0)
            return Enumerable.Empty<IntervalSummary>();

        // Create limits
        var cvAllowed = 0.10f;
        var deviationAllowed = 0.10f;
        var minPower = _powerZones.Values[5].LowLimit;

        var result = new List<IntervalSummary>();

        LogEventHandler?.Invoke(this, "Searching high power intervals...");
        var calculator = new MovingAveragesMetricsService();
        var powerModels = calculator
            .Smooth(records
                .Select(r => new SmoothInput(r.Timestamp, (float)(r.Performance?.Power ?? 0)))
                .ToList(),
            WindowSize);

        int indexModels = 0;
        while (indexModels < powerModels.Count)
        {
            // Look for start
            while (
                indexModels < powerModels.Count &&
                (powerModels[indexModels].CoefficientOfVariation > cvAllowed ||
                powerModels[indexModels].Average < minPower))
                indexModels++;

            if (indexModels >= powerModels.Count)
                break;

            var startIndexRecords = powerModels[indexModels].Index - (WindowSize - 1);
            var startTime = records.ElementAt(startIndexRecords).Timestamp;
            float referenceAverage = powerModels[indexModels].Average;
            int totalPower = (int)referenceAverage;
            int pointCount = 1;
            bool sessionStopped = false;
            indexModels++;
            LogEventHandler?.Invoke(this, $"New interval might start at: startDate={startTime.TimeOfDay} Average={powerModels[indexModels].Average}. CV={powerModels[indexModels].CoefficientOfVariation}");

            // Keep with the interval while it mantains stable
            int lowCount = 0;
            float lowAverage = 0;
            int lastAllowedPointRecords = 0;
            while (indexModels < powerModels.Count)
            {
                int timeDiff = (indexModels > 0) ? 
                    (int)(powerModels[indexModels].LastPoint.Timestamp - powerModels[indexModels - 1].LastPoint.Timestamp).TotalSeconds : 1;
                if (timeDiff > 1)
                {
                    LogEventHandler?.Invoke(this, $"Session stopped at {powerModels[indexModels - 1].LastPoint.Timestamp.TimeOfDay} for {timeDiff - WindowSize} seconds. Finishing interval");
                    sessionStopped = true;
                    break;
                }

                powerModels[indexModels].DeviationFromReference = (powerModels[indexModels].Average - referenceAverage) / referenceAverage;

                if (powerModels[indexModels].CoefficientOfVariation > cvAllowed ||
                    powerModels[indexModels].DeviationFromReference < 0 && Math.Abs(powerModels[indexModels].DeviationFromReference) >= deviationAllowed ||
                    powerModels[indexModels].Average < minPower)
                {
                    // Warning, might finish the interval
                    if (lowCount == 0)  // First warning
                    {
                        // Check from the beggining of the window to see what's wrong
                        for (int j = WindowSize - 1; j >= 0; j--)
                        {
                            int dangerIndexRecords = powerModels[indexModels].Index - j;
                            if (records.ElementAt(dangerIndexRecords).Performance?.Power < minPower)
                            {
                                lastAllowedPointRecords = dangerIndexRecords - 1;
                                LogEventHandler?.Invoke(
                                    this,
                                    $"Unstable point found at {records.ElementAt(dangerIndexRecords).Timestamp.TimeOfDay} ({indexModels}): Power={records.ElementAt(dangerIndexRecords).Performance?.Power}W");
                                lowAverage = records
                                    .Skip(lastAllowedPointRecords + 1)
                                    .Take(j + 1)
                                    .Select(r => r.Performance?.Power)
                                    .Where(p => p.HasValue)
                                    .Average(p => p!.Value);
                                lowCount = j + 1;
                                break;
                            }
                        }
                    }
                    // Once it enters in the danger zone (CV or deviation not allowed, and below minPower),
                    // CV and deviation must be restored to exit from it, is not enought to be above minPower
                    else
                    {
                        lowAverage = ((lowAverage * lowCount) + powerModels[indexModels].LastPoint.Value) / (lowCount + 1);
                        lowCount++;

                        // Being above the minimum power doesn't allow to exit the danger zone,
                        // but won't check if the interval has to end
                        if (powerModels[indexModels].LastPoint.Value < minPower)
                        {
                            var deviation = Math.Abs(lowAverage - referenceAverage) / referenceAverage;
                            if (deviation > 0.50f ||
                                (deviation > 0.25f && lowCount > 5) ||
                                (deviation > 0.10f && lowCount > 15) ||
                                lowCount > 30)
                            {
                                LogEventHandler?.Invoke(
                                   this,
                                   $"Interval finished. {lowCount} seconds with a deviation of {deviation}");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (lowCount > 0)
                    {
                        lowCount = 0;
                        lowAverage = 0;
                        lastAllowedPointRecords = 0;
                        LogEventHandler?.Invoke(
                           this,
                           $"Unstable points end at {powerModels[indexModels].LastPoint.Timestamp.TimeOfDay} ({indexModels}): Power={powerModels[indexModels].Average}");
                    }
                    totalPower += (int)powerModels[indexModels].LastPoint.Value;
                    pointCount++;
                    referenceAverage = totalPower / pointCount;
                }
                indexModels++;
            }

            int auxIndexRecords = lowCount > 0 ?
                lastAllowedPointRecords : 
                indexModels >= powerModels.Count ? records.Count() - 1 : powerModels[indexModels].Index;

            // Get refined limits
            var endTime = records.ElementAt(auxIndexRecords).Timestamp;
            var refinedStartIndex = GetStartIndex(records, startTime, endTime) + startIndexRecords;

            // Check if it can be considered an interval
            var newInterval = IntervalSummary.Create(
                records.ElementAt(refinedStartIndex).Timestamp,
                endTime, 
                records);
            if (IsConsideredAHighPowerInterval(newInterval))
            {
                result.Add(newInterval);
                LogEventHandler?.Invoke(
                    this,
                    $"Saved new interval at {newInterval.StartTime.TimeOfDay}, {newInterval.DurationSeconds}s at {newInterval.AveragePower}W");
            }
            else
            {
                LogEventHandler?.Invoke(
                    this,
                    $"Interval at {newInterval.StartTime.TimeOfDay}, {newInterval.DurationSeconds}s at {newInterval.AveragePower}W not saved");
            }
        }
        return result;
    }

    private int GetStartIndex(
        IEnumerable<RecordData> records, 
        DateTime startTime, 
        DateTime endTime)
    {
        var intervalRecords = records
                .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
                .ToList();

        var expectedAverage = intervalRecords
            .Skip(10)
            .SkipLast(10)
            .Select(r => r.Performance?.Power)
            .Average();

        int startIndex = 0;
        for (int i = 0; i < WindowSize; i++)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= expectedAverage)
            {
                startIndex = i;
                break;
            }
        }

        return startIndex;
    }

    private bool IsConsideredAHighPowerInterval(IntervalSummary interval)
    {
        return interval.DurationSeconds >= 30 &&
            interval.AveragePower >= _powerZones.Values[5].LowLimit;
    }
}

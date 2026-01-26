using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Services.Utils.Smoothers.Metrics;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;
using static Yact.Domain.Services.Utils.Smoothers.Metrics.MovingAveragesMetricsService;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal abstract class IntervalsFinder
{
    public event EventHandler<string>? LogEventHandler;

    protected readonly PowerZones _powerZones;
    protected readonly int _windowSize;
    protected readonly float _cvAllowed;
    protected readonly float _deviationAllowed;
    protected readonly float _deltaAllowed;

    protected List<MovingAverageMetric<SmoothInput>> _powerModels;
    protected IEnumerable<RecordData> _records;

    protected int _dangerCount = 0;
    protected int _dangerTotalPower = 0;
    protected float _referenceAverage = 0f;
    protected int _lastAllowedRecordsIndex = 0;

    protected IntervalsFinder(
        IEnumerable<RecordData> records,
        PowerZones powerZones,
        int windowSize,
        float cvAllowed, 
        float deviationAllowed,
        float deltaAllowed)
    {
        _records = records;
        _powerZones = powerZones;
        _cvAllowed = cvAllowed;
        _deviationAllowed = deviationAllowed;
        _windowSize = windowSize;
        _powerModels = new();
        _deltaAllowed = deltaAllowed;
    }

    internal virtual IEnumerable<IntervalSummary> Search()
    {
        // Check input
        if (_records.Count() == 0)
            return Enumerable.Empty<IntervalSummary>();

        var result = new List<IntervalSummary>();
        GetMovingAverages();

        RaiseLogEvent("Searching intervals...");

        int indexModels = 0;
        while (indexModels < _powerModels.Count)
        {
            // Look for start
            while (indexModels < _powerModels.Count && !IsIntervalStart(indexModels))
                indexModels++;

            if (indexModels >= _powerModels.Count)
                break;

            var startIndexRecords = _powerModels[indexModels].Index - (_windowSize - 1);
            var startTime = _records.ElementAt(startIndexRecords).Timestamp;
            _referenceAverage = _powerModels[indexModels].Average;
            int totalPower = (int)_referenceAverage;
            int pointCount = 1;
            bool sessionStopped = false;
            indexModels++;
            RaiseLogEvent($"New interval might start at: startDate={startTime.TimeOfDay} Average={_powerModels[indexModels].Average}. CV={_powerModels[indexModels].CoefficientOfVariation}");

            // While it keeps stable
            _dangerCount = 0;
            _dangerTotalPower = 0;
            _lastAllowedRecordsIndex = 0;
            while (indexModels < _powerModels.Count)
            {
                int timeDiff = (indexModels > 0) ?
                    (int)(_powerModels[indexModels].LastPoint.Timestamp - _powerModels[indexModels - 1].LastPoint.Timestamp).TotalSeconds : 1;

                if (timeDiff > 1)
                {
                    RaiseLogEvent($"Session stopped at {_powerModels[indexModels - 1].LastPoint.Timestamp.TimeOfDay} for {timeDiff - _windowSize} seconds. Finishing interval");
                    sessionStopped = true;
                    break;
                }

                _powerModels[indexModels].DeviationFromReference = (_powerModels[indexModels].Average - _referenceAverage) / _referenceAverage;

                if (IsIntervalEndWarning(indexModels))
                {
                    // Warning, might finish the interval
                    if (_dangerCount == 0)  // First warning
                    {
                        HandleFirstWarningPoint(indexModels);
                    }
                    else
                    {
                        _dangerTotalPower += (int)_powerModels[indexModels].LastPoint.Value;
                        _dangerCount++;
                        float dangerAverage = _dangerTotalPower / _dangerCount;

                        if (HasToEndInterval(indexModels, dangerAverage))
                            break;
                    }
                }
                else
                {
                    if (_dangerCount > 0)
                    {
                        totalPower += _dangerTotalPower;
                        pointCount += _dangerCount;

                        _dangerTotalPower = 0;
                        _dangerCount = 0;
                        _lastAllowedRecordsIndex = 0;
                        RaiseLogEvent($"Unstable points end at {_powerModels[indexModels].LastPoint.Timestamp.TimeOfDay} ({indexModels}): Power={_powerModels[indexModels].Average}");
                    }
                    else
                    {
                        totalPower += (int)_powerModels[indexModels].LastPoint.Value;
                        pointCount++;
                    }
                    _referenceAverage = totalPower / pointCount;
                }
                indexModels++;
            }

            var refinedLimits = GetIntervalLimitDateTimes(indexModels, startIndexRecords);

            // Check if it can be considered an interval
            var newInterval = IntervalSummary.Create(
                refinedLimits.Item1,
                refinedLimits.Item2,
                _records);
            if (IsConsideredAnInterval(newInterval))
            {
                result.Add(newInterval);
                RaiseLogEvent($"Saved new interval at {newInterval.StartTime.TimeOfDay}, {newInterval.DurationSeconds}s at {newInterval.AveragePower}W");
            }
            else
            {
                RaiseLogEvent($"Interval at {newInterval.StartTime.TimeOfDay}, {newInterval.DurationSeconds}s at {newInterval.AveragePower}W not saved");
            }
        }
        return result;
    }

    protected virtual void GetMovingAverages()
    {
        var calculator = new MovingAveragesMetricsService();
        _powerModels = calculator
            .Smooth(_records
                .Select(r => new SmoothInput(r.Timestamp, (float)(r.Performance?.Power ?? 0)))
                .ToList(),
            _windowSize);
    }

    protected virtual bool IsIntervalStart(int index)
    {
        return false;
    }

    protected virtual bool IsIntervalEndWarning(int index)
    {
        return false;
    }

    protected virtual void HandleFirstWarningPoint(int index)
    {
        if (_records == null)
            throw new NoDataException();

        // Check from the beggining of the window to see what's wrong
        for (int j = _windowSize - 1; j >= 0; j--)
        {
            int dangerIndexRecords = _powerModels[index].Index - j;
            if (_records.ElementAt(dangerIndexRecords).Performance?.Power < _referenceAverage)
            {
                _lastAllowedRecordsIndex = dangerIndexRecords - 1;
                LogEventHandler?.Invoke(
                    this,
                    $"Unstable point found at {_records.ElementAt(dangerIndexRecords).Timestamp.TimeOfDay} ({index}): Power={_records.ElementAt(dangerIndexRecords).Performance?.Power}W");
                _dangerTotalPower = (int)_records
                    .Skip(_lastAllowedRecordsIndex + 1)
                    .Take(j + 1)
                    .Select(r => r.Performance?.Power)
                    .Where(p => p.HasValue)
                    .Sum(p => p!.Value);
                _dangerCount = j + 1;
                break;
            }
        }
    }

    protected virtual bool HasToEndInterval(int indexModels, float dangerAverage)
    {
        var deviation = Math.Abs(dangerAverage - _referenceAverage) / _referenceAverage;
        if (deviation > 0.50f ||
            (deviation > 0.25f && _dangerCount > 5) ||
            (deviation > 0.10f && _dangerCount > 15) ||
            _dangerCount > 30)
        {
            RaiseLogEvent($"Interval finished. {_dangerCount} seconds with a deviation of {deviation}");
            return true;
        }
        else
            return false;
    }

    protected virtual (DateTime, DateTime) GetIntervalLimitDateTimes(int currentIndexModels, int startIndexRecords)
    {
        if (_records == null)
            throw new NoDataException();

        int auxIndexRecords = _dangerCount > 0 ?
                _lastAllowedRecordsIndex :
                currentIndexModels >= _powerModels.Count ? _records.Count() - 1 : _powerModels[currentIndexModels].Index;

        // Get refined limits
        var startTime = _records.ElementAt(startIndexRecords).Timestamp;
        
        var refinedEndTime = _records.ElementAt(auxIndexRecords).Timestamp;
        var refinedStartIndex = GetStartIndex(_records, startTime, refinedEndTime) + startIndexRecords;
        var refinedStartTime = _records.ElementAt(refinedStartIndex).Timestamp;

        return (refinedStartTime, refinedEndTime);
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
        for (int i = 0; i < _windowSize; i++)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= expectedAverage)
            {
                startIndex = i;
                break;
            }
        }

        return startIndex;
    }

    protected abstract bool IsConsideredAnInterval(IntervalSummary interval);

    protected void RaiseLogEvent(string message)
    {
        LogEventHandler?.Invoke(this, message);
    }
}

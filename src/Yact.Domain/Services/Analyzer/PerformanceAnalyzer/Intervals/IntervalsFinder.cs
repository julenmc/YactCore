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
    protected readonly int _minPower;
    protected readonly int _maxPower;
    protected readonly int _minTime;

    protected List<MovingAverageMetric<SmoothInput>> _powerModels;
    protected IEnumerable<RecordData> _records;

    protected int _dangerCount = 0;
    protected int _dangerTotalPower = 0;
    protected float _referenceAverage = 0f;
    protected int _lastAllowedRecordsIndex = 0;
    protected int _startIndexRecords = 0;
    protected DateTime _startTime;

    protected IntervalsFinder(
        IEnumerable<RecordData> records,
        PowerZones powerZones,
        int windowSize,
        int minTime,
        float cvAllowed, 
        float deviationAllowed,
        float deltaAllowed = 0,
        int minPower = 0,
        int maxPower = 2000)
    {
        _records = records;
        _powerZones = powerZones;
        _cvAllowed = cvAllowed;
        _deviationAllowed = deviationAllowed;
        _windowSize = windowSize;
        _powerModels = new();
        _minTime = minTime;
        _deltaAllowed = deltaAllowed;
        _minPower = minPower;
        _maxPower = maxPower;
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
            for (; indexModels < _powerModels.Count; indexModels++)
                if (IsIntervalStart(indexModels))
                    break;

            if (indexModels >= _powerModels.Count)
                break;

            _startIndexRecords = _powerModels[indexModels].Index - (_windowSize - 1);
            _startTime = _records.ElementAt(_startIndexRecords).Timestamp;
            _referenceAverage = _powerModels[indexModels].Average;
            int totalPower = (int)_referenceAverage * _windowSize;
            int pointCount = _windowSize;
            bool sessionStopped = false;
            indexModels++;
            RaiseLogEvent($"New interval might start at: startDate={_startTime.TimeOfDay} Average={_powerModels[indexModels].Average}. CV={_powerModels[indexModels].CoefficientOfVariation}");

            // While it keeps stable
            _dangerCount = 0;
            _dangerTotalPower = 0;
            _lastAllowedRecordsIndex = 0;
            for (; indexModels < _powerModels.Count; indexModels++)
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
                    if (_dangerCount == 0)  
                    {
                        HandleFirstWarningPoint(indexModels);
                    }
                    else
                    {
                        _dangerTotalPower += (int)_powerModels[indexModels].LastPoint.Value;
                        _dangerCount++;
                    }

                    if (_dangerCount > 0)
                    {
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
                        RaiseLogEvent($"Unstable points end at {_powerModels[indexModels].LastPoint.Timestamp.TimeOfDay} ({_powerModels[indexModels].Index}): Power={_powerModels[indexModels].LastPoint.Value}W");
                    }
                    else
                    {
                        totalPower += (int)_powerModels[indexModels].LastPoint.Value;
                        pointCount++;
                    }
                    _referenceAverage = totalPower / pointCount;
                }
            }

            var refinedLimits = GetIntervalLimitDateTimes(indexModels);

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

            if (indexModels < _powerModels.Count)
            {
                int? lastIntervalEndIndex = null;

                for (int index = 0; index < _powerModels.Count; index++) // TODO: optimize this search
                {
                    if (_powerModels[index].Index == _lastAllowedRecordsIndex + _windowSize)
                    {
                        lastIntervalEndIndex = index;
                        break;
                    }
                }

                if (lastIntervalEndIndex != null)
                    indexModels = lastIntervalEndIndex.Value;
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
        return _powerModels[index].CoefficientOfVariation <= _cvAllowed &&
            _powerModels[index].RangePercent <= _deltaAllowed &&
            _powerModels[index].Average >= _minPower &&
            _powerModels[index].Average <= _maxPower;
    }

    protected virtual bool IsIntervalEndWarning(int index)
    {
        return _powerModels[index].CoefficientOfVariation > _cvAllowed ||
            Math.Abs(_powerModels[index].DeviationFromReference) >= _deviationAllowed ||
            _powerModels[index].Average < _minPower;
    }

    protected virtual void HandleFirstWarningPoint(int index)
    {
        // Check from the beggining of the window to see what's wrong
        for (int j = _windowSize - 1; j >= 0; j--)
        {
            int dangerIndexRecords = _powerModels[index].Index - j;
            if (_records.ElementAt(dangerIndexRecords).Performance?.Power < _referenceAverage * (1 - _deviationAllowed) ||
                _records.ElementAt(dangerIndexRecords).Performance?.Power > _referenceAverage * (1 + _deviationAllowed))
            {
                _lastAllowedRecordsIndex = dangerIndexRecords - 1;
                LogEventHandler?.Invoke(
                    this,
                    $"Unstable point found at {_records.ElementAt(dangerIndexRecords).Timestamp.TimeOfDay} ({dangerIndexRecords}): " +
                    $"Average power => {_referenceAverage}W, point power => {_records.ElementAt(dangerIndexRecords).Performance?.Power}W");
                _dangerTotalPower = (int)_records
                    .Skip(_lastAllowedRecordsIndex + 1)
                    .Take(j + 1)
                    .Select(r => r.Performance?.Power)
                    .Where(p => p.HasValue)
                    .Sum(p => p!.Value);
                _dangerCount = j + 1;
                _referenceAverage = _records
                    .Skip(_startIndexRecords)
                    .Take(_lastAllowedRecordsIndex - _startIndexRecords)
                    .Select(r => r.Performance?.Power)
                    .Where(p => p.HasValue)
                    .Average(p => p!.Value);
                break;
            }
        }
    }

    private (DateTime, DateTime) GetIntervalLimitDateTimes(int currentIndexModels)
    {
        int auxIndexRecords = _dangerCount > 0 ?
                _lastAllowedRecordsIndex != _startIndexRecords ? 
                    _lastAllowedRecordsIndex : _startIndexRecords + 1 
                : currentIndexModels >= _powerModels.Count ? 
                    _records.Count() - 1 : _powerModels[currentIndexModels].Index;

        // Get refined limits
        var startTime = _records.ElementAt(_startIndexRecords).Timestamp;
        var endTime = _records.ElementAt(auxIndexRecords).Timestamp;

        var refinedStartIndex = GetStartIndex(startTime, endTime) + _startIndexRecords;
        var refinedStartTime = _records.ElementAt(refinedStartIndex).Timestamp;

        var refinedEndIndex = GetEndIndex(startTime, endTime) + _startIndexRecords;
        var refinedEndTime = _records.ElementAt(refinedEndIndex).Timestamp;

        return (refinedStartTime, refinedEndTime);
    }

    protected virtual int GetStartIndex(
        DateTime startTime,
        DateTime endTime)
    {
        var intervalRecords = _records
                .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
                .ToList();

        var expectedAverage = intervalRecords
            .Skip(_windowSize)
            .SkipLast(_windowSize)
            .Select(r => r.Performance?.Power)
            .Average();

        int startIndex = 0;
        for (int i = 0; i < intervalRecords.Count; i++)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= expectedAverage * (1 - _deviationAllowed) &&
                intervalRecords.ElementAt(i).Performance?.Power <= expectedAverage * (1 + _deviationAllowed))
            {
                startIndex = i;
                break;
            }
        }

        return startIndex;
    }

    protected virtual int GetEndIndex(
        DateTime startTime,
        DateTime endTime)
    {
        var intervalRecords = _records
        .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
        .ToList();

        var expectedAverage = intervalRecords
            .Skip(_windowSize)
            .SkipLast(_windowSize)
            .Select(r => r.Performance?.Power)
            .Average();

        int endIndex = 0;
        for (int i = intervalRecords.Count - 1; i >= 0; i--)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= expectedAverage * (1 - _deviationAllowed) &&
                intervalRecords.ElementAt(i).Performance?.Power <= expectedAverage * (1 + _deviationAllowed))
            {
                endIndex = i;
                break;
            }
        }

        return endIndex;
    }

    private bool IsConsideredAnInterval(IntervalSummary interval)
    {
        return interval.DurationSeconds >= _minTime &&
            interval.AveragePower >= _minPower;
    }

    protected abstract bool HasToEndInterval(int indexModels, float dangerAverage);

    protected void RaiseLogEvent(string message)
    {
        LogEventHandler?.Invoke(this, message);
    }
}

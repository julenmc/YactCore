using Yact.Domain.Exceptions.Activity;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsHighPowerFinder : IntervalsFinder
{
    private const int MinTime = 30;
    private const int WindowSizeHighPower = 10;
    private const float CvAllowed = 0.1f;
    private const float DeviationAllowed = 0.1f;

    internal IntervalsHighPowerFinder(
        PowerZones powerZones,
        IEnumerable<RecordData> records) 
        : base(records, powerZones, WindowSizeHighPower, MinTime, CvAllowed, DeviationAllowed, minPower: powerZones.Values[5].LowLimit) 
    {
    }

    protected override bool IsIntervalStart(int index)
    {
        return _powerModels[index].CoefficientOfVariation <= _cvAllowed &&
               _powerModels[index].Average >= _minPower;
    }

    protected override bool IsIntervalEndWarning(int index)
    {
        return _powerModels[index].CoefficientOfVariation > _cvAllowed ||
            _powerModels[index].DeviationFromReference < 0 && Math.Abs(_powerModels[index].DeviationFromReference) >= _deviationAllowed ||
            _powerModels[index].Average < _minPower;
    }

    protected override void HandleFirstWarningPoint(int index)
    {
        // Check from the beggining of the window to see what's wrong
        for (int j = _windowSize - 1; j >= 0; j--)
        {
            int dangerIndexRecords = _powerModels[index].Index - j;
            if (_records.ElementAt(dangerIndexRecords).Performance?.Power < _minPower)
            {
                _lastAllowedRecordsIndex = dangerIndexRecords - 1;
                RaiseLogEvent($"Unstable point found at {_records.ElementAt(dangerIndexRecords).Timestamp.TimeOfDay} ({index}): Power={_records.ElementAt(dangerIndexRecords).Performance?.Power}W");
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

    protected override bool HasToEndInterval(int indexModels, float dangerAverage)
    {
        // Being above the minimum power doesn't allow to exit the danger zone,
        // but won't check if the interval has to end
        if (_powerModels[indexModels].LastPoint.Value < _minPower)
        {
            var deviation = Math.Abs(dangerAverage - _minPower) / _minPower;
            if (deviation > 0.50f ||
                (deviation > 0.25f && _dangerCount > 5) ||
                (deviation > 0.10f && _dangerCount > 15) ||
                _dangerCount > 30)
            {
                RaiseLogEvent($"Interval finished. {_dangerCount} seconds with a deviation of {deviation}");
                return true;
            }
        }
        return false;
    }

    protected override int GetStartIndex(
        IEnumerable<RecordData> records,
        DateTime startTime,
        DateTime endTime)
    {
        var intervalRecords = records
                .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
                .ToList();

        var expectedAverage = intervalRecords
            .Skip(_windowSize)
            .SkipLast(_windowSize)
            .Select(r => r.Performance?.Power)
            .Average();

        int startIndex = 0;
        for (int i = 0; i < _windowSize; i++)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= _minPower)
            {
                startIndex = i;
                break;
            }
        }

        return startIndex;
    }

    protected override int GetEndIndex(
        IEnumerable<RecordData> records,
        DateTime startTime,
        DateTime endTime)
    {
        var intervalRecords = records
        .Where(p => p.Timestamp >= startTime && p.Timestamp <= endTime)
        .ToList();

        var expectedAverage = intervalRecords
            .Skip(_windowSize)
            .SkipLast(_windowSize)
            .Select(r => r.Performance?.Power)
            .Average();

        int endIndex = 0;
        for (int i = intervalRecords.Count - 1; i >= intervalRecords.Count - _windowSize; i--)
        {
            if (intervalRecords.ElementAt(i).Performance?.Power >= _minPower)
            {
                endIndex = i;
                break;
            }
        }

        return endIndex;
    }
}

using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsHighDurationFinder : IntervalsFinder
{
    private const int MinTime = 20 * 60;    // 20 mins
    private const int WindowSizeHighDuration = 60;
    private const float CvAllowed = 0.2f;
    private const float DeltaMax = 0.35f;
    private const float DeviationAllowed = 0.2f;

    internal IntervalsHighDurationFinder(
        PowerZones powerZones,
        IEnumerable<RecordData> records)
        : base(records, powerZones, WindowSizeHighDuration, MinTime, CvAllowed, DeviationAllowed, DeltaMax,
            minPower: (powerZones.Values[2].HighLimit + powerZones.Values[2].LowLimit) / 2,
            maxPower: powerZones.Values[5].LowLimit)
    {
    }

    protected override bool HasToEndInterval(int indexModels, float dangerAverage)
    {
        // Being inside the power limits doesn't allow to exit the danger zone,
        // but won't check if the interval has to end
        if (_powerModels[indexModels].LastPoint.Value < _referenceAverage * (1 - _deviationAllowed) ||
            _powerModels[indexModels].LastPoint.Value > _referenceAverage * (1 + _deviationAllowed))
        {
            var deviation = Math.Abs(dangerAverage - _referenceAverage) / _referenceAverage;
            if ((deviation > 0.75f && _dangerCount > 5) ||
                (deviation > 0.50f && _dangerCount > 30) ||
                (deviation > 0.25f && _dangerCount > 120))
            {
                RaiseLogEvent($"Interval finished. {_dangerCount} seconds with a deviation of {deviation}");
                return true;
            }
            else if (dangerAverage < _minPower)
            {
                var deviationFromMin = Math.Abs(dangerAverage - _minPower) / _minPower;
                if ((deviationFromMin > 0.75f && _dangerCount > 5) ||
                    (deviationFromMin > 0.50f && _dangerCount > 30) ||
                    (deviationFromMin > 0.25f && _dangerCount > 60) ||
                    (_dangerCount > 90))
                {
                    RaiseLogEvent($"Interval finished. {_dangerCount} seconds with a deviation of {deviationFromMin} from minimum");
                    return true;
                }
            }
        }
        return false;
    }
}

using Yact.Domain.Common.Activities.Intervals;
using Yact.Domain.Services.Utils.Smoothers.Metrics;
using Yact.Domain.ValueObjects.Activity.Intervals;
using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Common;
using Yact.Domain.ValueObjects.Cyclist;
using static Yact.Domain.Services.Utils.Smoothers.Metrics.MovingAveragesMetricsService;

namespace Yact.Domain.Services.Analyzer.PerformanceAnalyzer.Intervals;

internal class IntervalsFinder
{
    private readonly List<string> _debugTrace = new();
    private readonly IDictionary<int, Zone> _powerZones;
    private readonly Thresholds _thresholds;
    IntervalSearchGroups _searchGroup;
    private readonly int _windowSize;
    private readonly int _intervalStartMinPower;
    private readonly Zone _startThresholdPowerZone;
    private readonly IEnumerable<IntervalData> _container;

    internal IntervalsFinder(
        IDictionary<int, Zone> powerZones,
        IntervalSearchGroups searchGroup,
        IEnumerable<IntervalData>? container = null,
        Thresholds? thresholds = null)
    {
        _powerZones = powerZones;
        _searchGroup = searchGroup;
        _windowSize = IntervalTimes.IntervalSearchWindows[_searchGroup];

        try
        {
            var highZone = powerZones[IntervalZones.SearchRequiredZones[_searchGroup] + 2];
            var lowZone = powerZones[IntervalZones.SearchRequiredZones[_searchGroup]];
            _startThresholdPowerZone = new Zone(lowZone.LowLimit, highZone.HighLimit);
            _intervalStartMinPower = (lowZone.HighLimit + lowZone.LowLimit) / 2;
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }

        if (container != null)
            _container = container;
        else
            _container = new List<IntervalData>();

        if (thresholds != null)
            _thresholds = thresholds;
        else
        {
            _thresholds = _searchGroup switch
            {
                IntervalSearchGroups.Short => IntervalSearchValues.ShortIntervals.Default,
                IntervalSearchGroups.Medium => IntervalSearchValues.MediumIntervals.Default,
                _ => IntervalSearchValues.LongIntervals.Default,    // Long. If Long is writte, it forces to write "_" (default) case
            };
        }
    }

    internal List<string> GetDebugTrace() => _debugTrace;

    internal IEnumerable<IntervalData> Search(IEnumerable<RecordData> records)
    {
        _debugTrace.Clear();

        // Inicializar con valores por defecto si no se proporcionan
        List<IntervalData> result = new List<IntervalData>();
        float cvStartThr = _thresholds.CvStart;
        float cvFollowThr = _thresholds.CvFollow;
        float rangeThr = _thresholds.Range;
        float maRelThr = _thresholds.MaRel;

        int minStartPower = _intervalStartMinPower;
        int maxStartPower = _startThresholdPowerZone.HighLimit;

        _debugTrace.Add($"Using thresholds: cvStart={cvStartThr}, cvFollow={cvFollowThr}, range={rangeThr}, maRel={maRelThr}. minStartPower={minStartPower} maxStartPower={maxStartPower}W");

        // Calcular medias móviles para diferentes ventanas de tiempo
        _debugTrace.Add("Calculating moving averages...");
        var calculator = new MovingAveragesMetricsService();
        var powerModels = calculator
            .Smooth(records
                .Select(r => new SmoothInput(r.Timestamp, (float)(r.Performance?.Power ?? 0)))
                .ToList(), 
            _windowSize);
        _debugTrace.Add($"Generated {powerModels.Count} power models");

        // Buscar intervalos
        int i = 0;
        while (i < powerModels.Count)
        {
            // Buscar inicio de intervalo potencial
            while (i < powerModels.Count && !IsIntervalStart(powerModels[i], cvStartThr, rangeThr, maxStartPower, minStartPower))
                i++;

            if (i >= powerModels.Count)
                break;

            var startTime = powerModels[i].Timestamp;
            float referenceAverage = powerModels[i].Average;
            int totalPower = 0;
            int pointCount = 0;
            bool sessionStopped = false;
            int firstUnstablePointIndex = 0;
            _debugTrace.Add($"New interval might start at: startDate={startTime.TimeOfDay}: CV={powerModels[i].CoefficientOfVariation}. Range={powerModels[i].RangePercent}");

            // Seguir el intervalo mientras se mantenga estable
            int unstableCount = 0;
            while (i < powerModels.Count)
            {
                int timeDiff = (i > 0) ? (int)(powerModels[i].Timestamp - powerModels[i - 1].Timestamp).TotalSeconds : 1;
                if (timeDiff > 1)
                {
                    _debugTrace.Add($"Session stopped at {powerModels[i - 1].Timestamp.TimeOfDay} for {timeDiff - _windowSize} seconds. Finishing interval");
                    sessionStopped = true;
                    break;
                }
                var current = powerModels[i];
                current.DeviationFromReference = Math.Abs(current.Average - referenceAverage) / referenceAverage;

                if (!IsIntervalContinuation(current, cvFollowThr, maRelThr))
                {
                    if (unstableCount == 0)
                    {
                        _debugTrace.Add($"Unstable point found at {powerModels[i].Timestamp.TimeOfDay} ({i}): CV={current.CoefficientOfVariation}, Deviation={current.DeviationFromReference}");
                        firstUnstablePointIndex = i;
                    }
                    unstableCount++;
                    if (unstableCount >= _windowSize)
                    {
                        break;
                    }
                    pointCount++;
                    totalPower += (int)current.Average;
                }
                else
                {
                    if (unstableCount != 0)
                    {
                        _debugTrace.Add($"Unstable point ends at {powerModels[i].Timestamp.TimeOfDay} ({i}): CV={current.CoefficientOfVariation}, Deviation={current.DeviationFromReference}. Count: {unstableCount}");
                        unstableCount = 0;
                    }
                    pointCount++;
                    totalPower += (int)current.Average;
                    referenceAverage = (float)totalPower / pointCount;
                }

                i++;
            }

            int auxIndex = i >= powerModels.Count ? i - 1 : i;
            if (sessionStopped)
            {
                auxIndex--;
                i++;
            }
            i = (i < powerModels.Count && !sessionStopped) ? firstUnstablePointIndex + 1 : i;
            var endTime = powerModels[Math.Max(0, auxIndex)].Timestamp;

            var newInterval = new IntervalData
            {
                StartTime = startTime,
                EndTime = endTime,
                AveragePower = (int)referenceAverage,
            };

            // Refinar los límites del intervalo
            var refined = RefineIntervalLimits(newInterval, records.ToList());
            if (refined == null)
                continue;

            _debugTrace.Add($"Found interval: Time={refined.StartTime.TimeOfDay}-{refined.EndTime.TimeOfDay} ({refined.DurationSeconds}s), avgPower={refined.AveragePower}W");

            if (refined.DurationSeconds >= IntervalTimes.IntervalMinTimes[_searchGroup] && refined.IsConsideredAnInterval(_powerZones))
            {
                if (_container.Any(i => i == refined))
                {
                    _debugTrace.Add($"Interval not saved. Already exists.");
                }
                else
                {
                    _debugTrace.Add($"Saved interval: Time={refined.StartTime.TimeOfDay}-{refined.EndTime.TimeOfDay} ({refined.DurationSeconds}s), avgPower={refined.AveragePower}W");
                    result.Add(refined);
                }
            }
            else _debugTrace.Add($"Interval not saved. Not valid.");
        }
        return result;
    }

    private bool IsIntervalStart(MovingAverageMetric point, float cvThreshold, float rangeThreshold, float maxAvgPower, float minAvgPower)
    {
        //_debugTrace.Add($"Checking interval start at {point.Timestamp}: CV={point.CoefficientOfVariation}, Range={point.RangePercent}");

        return point.CoefficientOfVariation <= cvThreshold &&
               point.RangePercent <= rangeThreshold &&
               point.Average <= maxAvgPower &&
               point.Average >= minAvgPower;
    }

    private bool IsIntervalContinuation(MovingAverageMetric point, float cvThreshold, float deviationThreshold)
    {
        //_debugTrace.Add($"Checking interval continuation at {point.Timestamp}: CV={point.CoefficientOfVariation}, Deviation={point.DeviationFromReference}");

        return point.CoefficientOfVariation <= cvThreshold &&
               point.DeviationFromReference <= deviationThreshold;
    }

    private IntervalData? RefineIntervalLimits(IntervalData interval, List<RecordData> points)
    {
        _debugTrace.Add("Refining interval limits");
        // Find index of the initial and final points of the interval in the list
        int intervalStartIdx = points.FindIndex(p => p.Timestamp >= interval.StartTime);
        int intervalEndIdx = points.FindLastIndex(p => p.Timestamp <= interval.EndTime);

        // Expand range to include older points
        int extraPoints = interval.DurationSeconds switch
        {
            >= IntervalTimes.LongIntervalMinTime => Math.Max(IntervalTimes.LongWindowSize, _windowSize),// * 3,
            >= IntervalTimes.MediumIntervalMinTime => Math.Max(IntervalTimes.MediumWindowSize, _windowSize),// * 3,
            _ => Math.Max(IntervalTimes.ShortWindowSize, _windowSize),// * 3
        };
        int expandedStartIdx = Math.Max(0, intervalStartIdx - extraPoints);
        int expandedEndIdx = Math.Min(points.Count - 1, intervalEndIdx + extraPoints);

        var expandedEndPoints = points.GetRange(intervalEndIdx, expandedEndIdx - intervalEndIdx + 1);
        int auxIndex = 1;
        while (auxIndex < expandedEndPoints.Count)
        {
            if ((int)(expandedEndPoints[auxIndex].Timestamp - expandedEndPoints[auxIndex - 1].Timestamp).TotalSeconds > 1)
            {
                expandedEndIdx = auxIndex + intervalEndIdx - 1;
                expandedEndPoints = points.GetRange(intervalEndIdx, expandedEndIdx - intervalEndIdx + 1);
                auxIndex = 1;
            }
            else
                auxIndex++;
        }

        var expandedPoints = points.GetRange(expandedStartIdx, expandedEndIdx - expandedStartIdx + 1);

        float targetPower = interval.AveragePower;
        float defaultMaRel = interval.DurationSeconds switch
        {
            >= IntervalTimes.LongIntervalMinTime => IntervalSearchValues.LongIntervals.Default.MaRel,
            >= IntervalTimes.MediumIntervalMinTime => IntervalSearchValues.MediumIntervals.Default.MaRel,
            _ => IntervalSearchValues.ShortIntervals.Default.MaRel
        };
        float maRelThr = _thresholds.MaRel;
        float allowedDeviation = targetPower * maRelThr;

        // Refine initial limit - search backwards from initial point
        var startIndex = expandedPoints.FindIndex(
            p => p.Timestamp >= interval.StartTime);

        while (startIndex > 0)
        {
            var prevPower = expandedPoints[startIndex - 1].Performance?.Power ?? 0;
            if (Math.Abs(prevPower - targetPower) > allowedDeviation)
                break;
            startIndex--;
        }

        // Look if the real start of the interval is inside the actual limits
        while (startIndex < expandedPoints.Count - 1 &&
               Math.Abs((expandedPoints[startIndex].Performance?.Power ?? 0) - targetPower) > allowedDeviation)
        {
            startIndex++;
        }

        // Refinar límite final - buscar hacia adelante desde el punto final
        var endIndex = expandedPoints.FindLastIndex(
            p => p.Timestamp <= interval.EndTime);

        // while (endIndex < expandedPoints.Count - 1)
        // {
        //     var nextPower = expandedPoints[endIndex + 1].Stats.Power ?? 0;
        //     if (Math.Abs(nextPower - targetPower) > allowedDeviation)
        //         break;
        //     endIndex++;
        // }

        // Look if the real start of the interval is inside the actual limits
        while (endIndex > startIndex &&
               Math.Abs((expandedPoints[endIndex].Performance?.Power ?? 0) - targetPower) > allowedDeviation)
        {
            endIndex--;
        }

        // Update limits if a valid range was found
        if (startIndex < endIndex)
        {
            var newStartTime = expandedPoints[startIndex].Timestamp;
            var newEndTime = expandedPoints[endIndex].Timestamp;
            var duration = (newEndTime - newStartTime).TotalSeconds + 1;

            if (duration >= IntervalTimes.IntervalMinTime)
            {
                // _debugTrace.Add($"Refined interval limits: {interval.StartTime} -> {newStartTime}, {interval.EndTime} -> {newEndTime}");
                // Calculate new average with new time range
                var powers = expandedPoints
                    .Skip(startIndex)
                    .Take(endIndex - startIndex + 1)
                    .Select(p => p.Performance?.Power ?? 0);
                return new IntervalData
                {
                    StartTime = newStartTime,
                    EndTime = newEndTime,
                    AveragePower = (int)powers.Average()
                };
            }
            return null;
        }
        else    // Means the interval is not valid, return an invalid interval so it can be refused
        {
            _debugTrace.Add($"Invalid interval, returning nule values. Original: {interval.StartTime.TimeOfDay}-{interval.EndTime.TimeOfDay} ({interval.DurationSeconds}s) at {interval.AveragePower}W");
            return null;
        }
    }
}
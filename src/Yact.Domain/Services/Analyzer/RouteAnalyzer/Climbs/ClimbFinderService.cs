using Yact.Domain.ValueObjects.Activity.Records;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

public class ClimbFinderService
{
    private enum Result
    {
        Continue,
        EndWarning,
        ClimbEnd,
        NoClimb
    }

    private const int SlopeAcceptanceValue = 2;

    private readonly List<string> _debugTrace;

    private bool _isFirstPoint = true;
    private double _climbStartDistance = 0;
    private double _forcePrevDistance = 0;
    private double _checkPrevDistance = 0;
    private List<RecordData> _climbRecords;

    public ClimbFinderService() 
    {
        _debugTrace = new List<string>();
        _climbRecords = new List<RecordData>();
    }

    public List<string> GetDebugTrace()
    {
        return _debugTrace;
    }

    public List<ClimbDetails> FindClimbs(List<RecordData> records)
    {
        if (records.Count() == 0)
            throw new ArgumentException($"No records given");

        _debugTrace.Clear();
        _climbRecords.Clear();
        var result = new List<ClimbDetails>();
        ClimbMetricsAccumulator climb = ClimbMetricsAccumulator.New();

        _isFirstPoint = true;
        _forcePrevDistance = 0;
        _checkPrevDistance = 0;
        double forceDistance = -1;
        Result r = Result.NoClimb;

        // Start at index 1 because first RecordData of activity will allways have Slope 0
        for (int i = 1; i < records.Count; i++)
        {
            if (records[i].DistanceMeters <= forceDistance)
            {
                _debugTrace.Add($"Forcing mount point at {Math.Round(records[i].DistanceMeters)}m until {Math.Round(forceDistance)}");
                ForcePoint(climb, records[i]);
                continue;
            }

            r = AddPoint(climb, records[i], records[i-1]);
            if (r == Result.EndWarning)
            {
                _debugTrace.Add($"Climb could end at {Math.Round(records[i].DistanceMeters)}m ({records[i].SmoothedAltitude.Altitude}m alt)");
                int index = i;
                r = Check(climb, records[index]);
                // Checks future points to see if there's a climb or not
                index++;
                if (index >= records.Count)
                    break;
                while (index < records.Count && r == Result.EndWarning)
                {
                    r = Check(climb, records[index]);
                    index++;
                }

                if (r == Result.ClimbEnd)
                {
                    _debugTrace.Add($"Climb ended at {Math.Round(records[i].DistanceMeters)}m ({records[i].SmoothedAltitude.Altitude}m alt)");
                    result.Add(EndClimb(climb));
                    climb = ClimbMetricsAccumulator.New();
                    _climbRecords.Clear();
                    _isFirstPoint = true;
                }
                else if (r == Result.Continue)
                {
                    _debugTrace.Add($"Climb continues at {Math.Round(records[index - 1].DistanceMeters)}m");
                    ForcePoint(climb, records[i]);
                    forceDistance = records[index - 1].DistanceMeters;
                }
            }
            else if (r == Result.NoClimb)
            {
                _debugTrace.Add($"No climb at {Math.Round(records[i].DistanceMeters)}m");
                _isFirstPoint = true;
                climb = ClimbMetricsAccumulator.New();
                _climbRecords.Clear();
            }
        }

        // If there was a climb being detected, check if its a climb and close and save it
        if (r == Result.Continue || r == Result.EndWarning)
        {
            if (IsClimb(climb, records.Last().SmoothedAltitude.Slope, records.Last().SmoothedAltitude.Altitude))
            {
                result.Add(EndClimb(climb));
            }
        }

        return result;
    }

    private Result AddPoint(ClimbMetricsAccumulator climb, RecordData record, RecordData prevRecord)
    {
        if (!_isFirstPoint)
        {
            double distDiff = record.DistanceMeters - _climbRecords.Last().DistanceMeters;
            double eleDiff = record.SmoothedAltitude.Altitude - _climbRecords.Last().SmoothedAltitude.Altitude;
            double slope = record.SmoothedAltitude.Slope;

            if (slope < SlopeAcceptanceValue)
            {
                return IsClimb(climb, slope, record.SmoothedAltitude.Altitude) ? Result.EndWarning : Result.NoClimb;
            }

            climb.Update(distDiff, eleDiff, slope);
            _climbRecords.Add(record);
            return Result.Continue;
        }
        else
        {
            if (record.SmoothedAltitude.Slope > SlopeAcceptanceValue)
            {
                _isFirstPoint = false;
                _climbRecords.Add(prevRecord);
                _climbRecords.Add(record);
                _climbStartDistance = prevRecord.DistanceMeters;
                climb.Update(
                    record.DistanceMeters - prevRecord.DistanceMeters,
                    record.SmoothedAltitude.Altitude - prevRecord.SmoothedAltitude.Altitude,
                    record.SmoothedAltitude.Slope);
                _debugTrace.Add($"New climb could start at {Math.Round(prevRecord.DistanceMeters)}");
                return Result.Continue;
            }
            else return Result.NoClimb;
        }
    }

    private void ForcePoint(ClimbMetricsAccumulator climb, RecordData record)
    {
        if (_forcePrevDistance < _climbRecords.Last().DistanceMeters)
            _forcePrevDistance = _climbRecords.Last().DistanceMeters;

        double distDiff = record.DistanceMeters - _forcePrevDistance;
        double eleDiff = record.SmoothedAltitude.Altitude - _climbRecords.Last().SmoothedAltitude.Altitude;
        double slope = record.SmoothedAltitude.Slope;
        _forcePrevDistance = record.DistanceMeters;

        climb.Update(distDiff, eleDiff, slope);
        _climbRecords.Add(record);
    }


    private Result Check(ClimbMetricsAccumulator climb, RecordData record)
    {
        if (_checkPrevDistance == 0)
        {
            _checkPrevDistance = _climbRecords.Last().DistanceMeters;
        }

        double distDiff = record.DistanceMeters - _checkPrevDistance;
        _checkPrevDistance = record.DistanceMeters;

        double slope = record.SmoothedAltitude.Slope; // Point slope
        double checkSlope = (record.SmoothedAltitude.Altitude - _climbRecords.Last().SmoothedAltitude.Altitude) /
            (record.DistanceMeters - _climbRecords.Last().DistanceMeters) * 100; // Slope in the sector to check

        bool hasToCheckAltitude = record.SmoothedAltitude.Altitude <= _climbRecords.Last().SmoothedAltitude.Altitude;
        bool hasToCheckSlope = record.SmoothedAltitude.Slope < SlopeAcceptanceValue;

        if (!hasToCheckAltitude && !hasToCheckSlope)
        {
            // All good, continue with the climb
            _checkPrevDistance = 0;
            return Result.Continue;
        }
        else if (!ContinueCheck(climb, record.DistanceMeters - _climbRecords.Last().DistanceMeters, checkSlope))
        {
            // Climb ended
            _checkPrevDistance = 0;
            return Result.ClimbEnd;
        }
        else
        {
            // Has to check more
            return Result.EndWarning;
        }
    }

    private bool IsClimb(ClimbMetricsAccumulator climb, double slope, double altitude)
    {
        for (int i = 0; i < ClimbRequirements.GetLength(0); i++)
        {
            if (climb.Slope >= ClimbRequirements[i, 0] && climb.DistanceMeters! >= ClimbRequirements[i,1])
                return true;
            if (i == ClimbRequirements.GetLength(0) - 1)
                return false;
        }

        return false;
    }

    private bool ContinueCheck(ClimbMetricsAccumulator climb, double distance, double slope)
    {
        int checkRow = slope < -3 ? 0 : 1;    // First row for downhills, second for flats
        double checkClimbDistance = 0;
        double checkStopDistance = 0;

        // Check max distance
        checkStopDistance = CheckLim[checkRow, CheckLim.GetLength(1) - 1, 1];
        if (distance > checkStopDistance)
            return false;

        // Check distances
        for (int i = 0; i < CheckLim.GetLength(1) - 1; i++)
        {
            checkClimbDistance = CheckLim[checkRow, i, 0];
            checkStopDistance = CheckLim[checkRow, i, 1];
            if (climb.DistanceMeters < checkClimbDistance && distance > checkStopDistance) 
                return false;
        }

        return true;
    }

    private ClimbDetails EndClimb(ClimbMetricsAccumulator climb)
    {
        var firstRecord = _climbRecords.First();
        var lastRecord = _climbRecords.Last();
        return new ClimbDetails()
        {
            Coordinates = new ClimbCoordinates
            { 
                LatitudeInit = firstRecord.Coordinates.Latitude,
                LatitudeEnd = lastRecord.Coordinates.Latitude,
                LongitudeInit = firstRecord.Coordinates.Longitude,
                LongitudeEnd = lastRecord.Coordinates.Longitude,
                AltitudeInit = firstRecord.SmoothedAltitude.Altitude,
                AltitudeEnd = lastRecord.SmoothedAltitude.Altitude,
            },
            Metrics = climb.Build(),
            StartPointMeters = _climbStartDistance
        };
    }

    private static readonly double[,] ClimbRequirements =
    {
        {2.5, 2000},
        {4.0, 1000},
        {5.0, 500},
        {6.0, 250},
        {7.0, 150},
        {8.0, 100},
        {10.0, 60}
    };

    private static readonly double[,,] CheckLim =
    {
        {   // downhill limits
            {2000, 200},        // Climb distance / Allowed downhill distances. Up to 2km, 200m of downhill are allowed
            {4000, 600},        // Up to 4km, 600m of downhill are allowed
            {6000, 800},
            {10000, 1000},
            {15000, 1200},
            {0, 2000},          // More than 2km of downhill won't be allowed
        },
        {   // flat distances
            {2000, 500},		// Climb distance / Allowed flat distances. Up to 2km, 500m of flat are allowed
			{4000, 800},        // Up to 4km, 1000m of flat are allowed
            {6000, 1000},
            {10000, 1500},
            {15000, 2000},
            {0, 3000}           // More than 3km of flat won't be allowed
        }
    };
}

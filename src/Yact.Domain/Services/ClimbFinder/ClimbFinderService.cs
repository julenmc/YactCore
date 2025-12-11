using Yact.Domain.Entities.Activity;
using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Services.ClimbFinder;

public class ClimbFinderService : IClimbFinderService
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
    private double _checkPrevAltitude = 0;
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

    public List<ActivityClimb> FindClimbs(List<RecordData> records)
    {
        if (records.Count() == 0)
            throw new ArgumentException($"No records given");

        _debugTrace.Clear();
        _climbRecords.Clear();
        List<ActivityClimb> result = new List<ActivityClimb>();
        ClimbMetrics climb = new ClimbMetrics();

        _isFirstPoint = true;
        _forcePrevDistance = 0;
        _checkPrevDistance = 0;
        _checkPrevAltitude = 0;
        double forceDistance = -1;
        Result r = Result.NoClimb;

        // Start at index 1 because first RecordData of activity will allways have Slope 0
        for (int i = 1; i < records.Count; i++)
        {
            if (records[i].DistanceMeters <= forceDistance)
            {
                _debugTrace.Add($"Forcing mount point at {Math.Round(records[i].DistanceMeters!.Value)}m until {Math.Round(forceDistance)}");
                ForcePoint(climb, records[i]);
                continue;
            }

            r = AddPoint(climb, records[i], records[i-1]);
            if (r == Result.EndWarning)
            {
                _debugTrace.Add($"Climb could end at {Math.Round(records[i].DistanceMeters!.Value)}m ({records[i].Coordinates.Altitude}m alt)");
                // Checks future points to see if there's a climb or not
                int index = i + 1;
                if (index >= records.Count)
                    break;
                r = Check(climb, records[index]);
                while (index < records.Count)
                {
                    if (r == Result.EndWarning)
                    {
                        index++;
                        if (index == records.Count - 1)     // If activity has ended checks if it has a port at the end
                        {
                            r = IsClimb(climb, 0, 0) ? Result.ClimbEnd : Result.NoClimb;
                            break;
                        }
                        r = Check(climb, records[index]);
                    }
                    else
                        break;
                }

                if (r == Result.ClimbEnd)
                {
                    _debugTrace.Add($"Climb ended at {Math.Round(records[i].DistanceMeters!.Value)}m ({records[i].Coordinates.Altitude}m alt)");
                    result.Add(EndClimb(climb));
                    climb = new ClimbMetrics();
                    _climbRecords.Clear();
                    _isFirstPoint = true;
                }
                else if (r == Result.Continue)
                {
                    _debugTrace.Add($"Climb continues at {Math.Round(records[index].DistanceMeters!.Value)}m");
                    ForcePoint(climb, records[i]);
                    forceDistance = records[index].DistanceMeters!.Value;
                }
            }
            else if (r == Result.NoClimb)
            {
                _debugTrace.Add($"No climb at {Math.Round(records[i].DistanceMeters!.Value)}m");
                _isFirstPoint = true;
                climb = new ClimbMetrics();
                _climbRecords.Clear();
            }
        }

        // If there was a climb being detected, check if its a climb and close and save it
        if (r == Result.Continue || r == Result.EndWarning)
        {
            if (IsClimb(climb, records.Last().Slope!.Value, records.Last().Coordinates.Altitude))
            {
                result.Add(EndClimb(climb));
            }
        }

        return result;
    }

    private Result AddPoint(ClimbMetrics climb, RecordData record, RecordData prevRecord)
    {
        if (!_isFirstPoint)
        {
            double distDiff = record.DistanceMeters!.Value - _climbRecords.Last().DistanceMeters!.Value;
            double slope = record.Slope!.Value;

            if (slope < SlopeAcceptanceValue)
            {
                return IsClimb(climb, slope, record.Coordinates.Altitude) ? Result.EndWarning : Result.NoClimb;
            }
            else if (slope > climb.MaxSlope)
            {
                climb.MaxSlope = slope;
            }

            climb.DistanceMeters += distDiff;
            if (record.Coordinates.Altitude > _climbRecords.Last().Coordinates.Altitude) 
                climb.Elevation += record.Coordinates.Altitude - _climbRecords.Last().Coordinates.Altitude;
            climb.Slope = (record.Coordinates.Altitude - _climbRecords.First().Coordinates.Altitude) / climb.DistanceMeters * 100;

            _climbRecords.Add(record);
            return Result.Continue;
        }
        else
        {
            if (record.Slope!.Value > SlopeAcceptanceValue)
            {
                _isFirstPoint = false;
                _climbRecords.Add(prevRecord);
                _climbRecords.Add(record);
                climb.Elevation = record.Coordinates.Altitude - prevRecord.Coordinates.Altitude;
                climb.DistanceMeters = record.DistanceMeters!.Value - prevRecord.DistanceMeters!.Value;
                climb.Slope = record.Slope!.Value;
                climb.MaxSlope = record.Slope!.Value;
                _climbStartDistance = prevRecord.DistanceMeters!.Value;
                _debugTrace.Add($"New climb could start at {Math.Round(prevRecord.DistanceMeters!.Value)}");
                return Result.Continue;
            }
            else return Result.NoClimb;
        }
    }

    private void ForcePoint(ClimbMetrics climb, RecordData record)
    {
        if (_forcePrevDistance < _climbRecords.Last().DistanceMeters!.Value)
            _forcePrevDistance = _climbRecords.Last().DistanceMeters!.Value;

        double distDiff = record.DistanceMeters!.Value - _forcePrevDistance;
        _forcePrevDistance = record.DistanceMeters!.Value;

        double slope = record.Slope!.Value;
        climb.DistanceMeters += distDiff;
        if (record.Coordinates.Altitude > _climbRecords.Last().Coordinates.Altitude)
            climb.Elevation += record.Coordinates.Altitude - _climbRecords.Last().Coordinates.Altitude;
        climb.Slope = (record.Coordinates.Altitude - _climbRecords.First().Coordinates.Altitude) / climb.DistanceMeters * 100;
        if (slope > climb.MaxSlope)
        {
            climb.MaxSlope = slope;
        }
        _climbRecords.Add(record);
    }


    private Result Check(ClimbMetrics climb, RecordData record)
    {
        if (_checkPrevDistance == 0)
        {
            _checkPrevDistance = _climbRecords.Last().DistanceMeters!.Value;
            _checkPrevAltitude = _climbRecords.Last().Coordinates.Altitude;
        }

        double distDiff = record.DistanceMeters!.Value - _checkPrevDistance;
        _checkPrevDistance = record.DistanceMeters!.Value;

        double slope = record.Slope!.Value; // Point slope
        double checkSlope = (record.Coordinates.Altitude - _climbRecords.First().Coordinates.Altitude) /
            (record.DistanceMeters!.Value - _climbRecords.First().DistanceMeters!.Value) * 100; // Slope in the climb with the point to check

        bool hasToCheckAltitude = record.Coordinates.Altitude < _climbRecords.Last().Coordinates.Altitude;
        bool hasToCheckSlope = record.Slope < SlopeAcceptanceValue;

        if (!hasToCheckAltitude && !hasToCheckSlope)
        {
            // All good, continue with the climb
            _checkPrevAltitude = 0;
            _checkPrevDistance = 0;
            return Result.Continue;
        }
        else if (!ContinueCheck(climb, record.DistanceMeters!.Value - _climbRecords.Last().DistanceMeters!.Value, checkSlope))
        {
            // Climb ended
            _checkPrevAltitude = 0;
            _checkPrevDistance = 0;
            return Result.ClimbEnd;
        }
        else
        {
            // Has to check more
            return Result.EndWarning;
        }
    }

    private bool IsClimb(ClimbMetrics climb, double slope, double altitude)
    {
        for (int i = 0; i < MountReq.GetLength(0); i++)
        {
            if ((climb.Slope >= MountReq[i, 0]) && (climb.DistanceMeters! >= MountReq[i,1]))
                return true;
            if (i == MountReq.GetLength(0) - 1)
                return false;
        }

        return false;
    }

    private bool ContinueCheck(ClimbMetrics climb, double distance, double slope)
    {
        int checkRow = (slope < -3) ? 0 : 1;    // First row for downhills, second for flats

        // Check distances
        for (int i = 0; i < CheckLim.GetLength(1) - 1; i++)
        {
            if ((climb.DistanceMeters < CheckLim[checkRow, i + 1, 0]) && (distance > CheckLim[checkRow, i, 1])) 
                return false;
        }

        // Check last distance
        if ((climb.DistanceMeters < CheckLim[checkRow, CheckLim.GetLength(0) - 1, 0]) && (distance > CheckLim[checkRow, CheckLim.GetLength(0), 1]))
            return false;

        return true;
    }

    private ActivityClimb EndClimb(ClimbMetrics climb)
    {
        return new ActivityClimb()
        {
            Data = new ClimbData 
            { 
                Metrics = new ClimbMetrics()
                {
                    DistanceMeters = Math.Round(climb.DistanceMeters),
                    Elevation = Math.Round(climb.Elevation),
                    Slope = Math.Round(climb.Slope, 1),
                    MaxSlope = Math.Round(climb.MaxSlope, 1)
                }
            },
            StartPointMeters = _climbStartDistance
        };
    }

    private static readonly double[,] MountReq =
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
            {0.0, 200},
            {2000, 600},
            {4000, 800},
            {6000, 900},
            {10000, 1200},
            {15000, 2000}
        },
        {   // flat distances
            {0.0, 500},
            {2000, 800},		// Mount distance / Allowed flat distances. Up to 2km 800m are allowed
			{4000, 1000},
            {6000, 1500},
            {10000, 2000},
            {15000, 3000}
        }
    };
}

using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

internal class ClimbMetricsAccumulator
{
    public double DistanceMeters
    {
        get { return Math.Round(_distance); }
    }
    public double Slope
    {
        get { return Math.Round(_slope, 1); }
    }
    public double MaxSlope
    {
        get { return Math.Round(_maxSlope, 1); }
    }
    public double NetElevationMeters
    {
        get { return Math.Round(_elevation); }
    }
    public double TotalElevationMeters
    {
        get { return Math.Round(_totalElevation); }
    }

    private double _distance;
    private double _elevation;
    private double _slope;
    private double _maxSlope;
    private double _totalElevation;

    private ClimbMetricsAccumulator()
    {
        _distance = 0;
        _slope = 0;
        _maxSlope = 0;
        _elevation = 0;
        _totalElevation = 0;
    }

    public static ClimbMetricsAccumulator New() 
        => new ClimbMetricsAccumulator();

    public ClimbMetrics Build()
        => new ClimbMetrics()
        {
            DistanceMeters = this.DistanceMeters,
            NetElevationMeters = this.NetElevationMeters,
            TotalElevationMeters = this.TotalElevationMeters,
            Slope = this.Slope,
            MaxSlope = this.MaxSlope
        };

    public void Update(
        double distance,
        double elevation,
        double slope)
    {
        _distance += distance;
        _elevation += elevation;
        if (elevation > 0)
        {
            _totalElevation += elevation;
        }
        if (slope > _maxSlope)
        {
            _maxSlope = slope;
        }

        _slope = _elevation / _distance * 100;
    }
}

using Yact.Domain.Entities.Activity;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;
using Yact.Domain.Services.Utils.Smoothers.Altitude;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer;

public class RouteAnalyzerService : IRouteAnalyzerService
{
    private readonly IClimbFinderService _climbFinderService;
    private readonly IDistanceCalculator _distanceCalculator;
    private readonly IAltitudeSmootherService _altitudeSmootherService;
    private readonly IClimbMatcherService _climbMatcherService;

    public RouteAnalyzerService(
        IClimbFinderService climbFinderService,
        IClimbMatcherService climbMatcherService,
        IDistanceCalculator distanceCalculator,
        IAltitudeSmootherService altitudeSmootherService)
    {
        _climbFinderService = climbFinderService;
        _climbMatcherService = climbMatcherService;
        _distanceCalculator = distanceCalculator;
        _altitudeSmootherService = altitudeSmootherService;
    }

    /// <summary>
    /// Method to perform a full analysis of the route. It will calculate distances and slopes for each record, 
    /// smooth the altitude and find climbs in the route.
    /// </summary>
    /// <param name="records">List of records read in the activity file. 
    /// This method will include the distances from the start, slopes and 
    /// smooth the altitude for each record.</param>
    /// <returns>List of climbs done in the activity</returns>
    public async Task<List<ActivityClimb>> AnalyzeRouteAsync(List<RecordData> records)
    {
        // Get distances, slopes and smoothed altitude
        GetRouteDistances(records);

        // Get the climbs
        var climbs = _climbFinderService.FindClimbs(records);
        // Check if found climbs already exist in the repository
        foreach (var climb in climbs)
        {
            await _climbMatcherService.MatchClimbWithRepositoryAsync(climb);
        }

        return climbs;
    }

    /// <summary>
    /// Method to calculate distances and slopes for each record and 
    /// smooth the altitude.
    /// </summary>
    /// <param name="records">List of records read in the activity file. 
    /// This method will include the distances from the start, slopes and 
    /// smooth the altitude for each record.</param>
    public void GetRouteDistances(List<RecordData> records)
    {
        // First distance calculator, the smoother needs the distances
        _distanceCalculator.CalculateDistances(records);
        _altitudeSmootherService.Smooth(records);
    }

    public List<string> GetDebugTrace()
    {
        return _climbFinderService.GetDebugTrace();
    }
}

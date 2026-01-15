using Microsoft.Extensions.Logging;
using Yact.Domain.Entities;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.Services.Activities;

public class ClimbHandlerService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IClimbRepository _climbRepository;
    private readonly ILogger<ClimbHandlerService> _logger;

    public ClimbHandlerService(
        IActivityRepository activityRepository,
        IClimbRepository climbRepository,
        ILogger<ClimbHandlerService> logger)
    {
        _activityRepository = activityRepository;
        _climbRepository = climbRepository;
        _logger = logger;
    }

    public async Task Execute(Activity activity)
    {
        if (activity.Records == null || activity.Records.Values.Count == 0)
        {
            throw new NoDataException();
        }

        // Search for climbs
        ClimbFinderService finder = new ClimbFinderService();
        var climbsDetails = finder.FindClimbs(activity.Records.Values.ToList());
        //var debugTrace = _climbFinderService.GetDebugTrace();
        _logger.LogInformation($"{climbsDetails.Count} climbs found:");

        // Check if found climbs already exist in the repository
        ClimbMatcherService matcher = new ClimbMatcherService(_climbRepository); 
        foreach (var climbDetails in climbsDetails)
        {
            var climb = await matcher.MatchClimbWithRepositoryAsync(climbDetails);
            if (climb == null)   
            {
                // Climb doesn't exist
                climb = Climb.Create(ClimbId.NewId(), climbDetails, new ClimbSummary("Unknown"));
                await _climbRepository.AddAsync(climb);
            }
            _logger.LogInformation($"{climbDetails.Metrics.DistanceMeters}m at {climbDetails.Metrics.Slope}%");

            // Save activity climb
            activity.AddClimb(climb.Id, climbDetails.StartPointMeters);
            await _activityRepository.UpdateAsync(activity);
        } 
    }
}

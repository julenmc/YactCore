using Microsoft.Extensions.Logging;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

namespace Yact.Application.Services.Activities;

public class ClimbHandlerService
{
    private readonly IClimbRepository _repository;
    private readonly IActivityClimbRepository _activityClimbRepository;
    private readonly ILogger<ClimbHandlerService> _logger;

    public ClimbHandlerService(
        IClimbRepository repository,
        IActivityClimbRepository activityClimbRepository,
        ILogger<ClimbHandlerService> logger)
    {
        _repository = repository;
        _activityClimbRepository = activityClimbRepository;
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
        var climbs = finder.FindClimbs(activity.Records.Values.ToList());
        //var debugTrace = _climbFinderService.GetDebugTrace();

        // Check if found climbs already exist in the repository
        ClimbMatcherService matcher = new ClimbMatcherService(_repository); 
        foreach (var climb in climbs)
        {
            await matcher.MatchClimbWithRepositoryAsync(climb);
        } 

        // Save non existing climbs
        _logger.LogInformation($"{climbs.Count} climbs found:");
        foreach (var climb in climbs)
        {
            if (climb.ClimbId == 0)     // Means it wasn't matched with an existing climb
            {
                var newClimb = await _repository.AddAsync(climb.Data);
                newClimb.Name = "Unknown";
                climb.MergeWith(newClimb);
            }
            _logger.LogInformation($"{climb.Data.Metrics.DistanceMeters}m at {climb.Data.Metrics.Slope}%");
        }

        // Save activity climbs
        foreach (var climb in climbs)
        {
            await _activityClimbRepository.AddAsync(climbs, activity.Id.Value);
        }
    }
}

using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

public class ClimbMatcherService : IClimbMatcherService
{
    private const float CoordinatesDelta = 0.01f;

    private readonly IClimbRepository _repository;

    public ClimbMatcherService(IClimbRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> MatchClimbWithRepositoryAsync(ActivityClimb climb)
    {
        var latitudeMin = climb.Data.LatitudeInit * (1 - CoordinatesDelta);
        var latitudeMax = climb.Data.LatitudeInit * (1 + CoordinatesDelta);
        var longitudeMin = climb.Data.LongitudeInit * (1 - CoordinatesDelta);
        var longitudeMax = climb.Data.LongitudeInit * (1 + CoordinatesDelta);

        var repoClimbList = await _repository.GetByCoordinates((float)latitudeMin, (float)latitudeMax, (float)longitudeMin, (float)longitudeMax);
        foreach (ClimbData item in repoClimbList)
        {
            if(climb.Data.Match(item))
            {
                climb.MergeWith(item);
                return true;
            }
        }

        return false;
    }
}

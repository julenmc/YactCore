using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

public class ClimbMatcherService
{
    private const float CoordinatesDelta = 0.01f;

    private readonly IClimbRepository _repository;

    public ClimbMatcherService(IClimbRepository repository)
    {
        _repository = repository;
    }

    public async Task<Climb?> MatchClimbWithRepositoryAsync(ClimbDetails climbData)
    {
        var latitudeMin = climbData.Coordinates.LatitudeInit * (1 - CoordinatesDelta);
        var latitudeMax = climbData.Coordinates.LatitudeInit * (1 + CoordinatesDelta);
        var longitudeMin = climbData.Coordinates.LongitudeInit * (1 - CoordinatesDelta);
        var longitudeMax = climbData.Coordinates.LongitudeInit * (1 + CoordinatesDelta);

        var repoClimbList = await _repository.GetByCoordinatesAsync((float)latitudeMin, (float)latitudeMax, (float)longitudeMin, (float)longitudeMax);
        foreach (var item in repoClimbList)
        {
            if(climbData.Match(item.Data))
            {
                return item;
            }
        }
        
        return null;
    }
}

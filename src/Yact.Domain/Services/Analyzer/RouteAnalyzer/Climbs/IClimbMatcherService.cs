using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

public interface IClimbMatcherService
{
    Task MatchClimbWithRepositoryAsync(ActivityClimb climb);
}

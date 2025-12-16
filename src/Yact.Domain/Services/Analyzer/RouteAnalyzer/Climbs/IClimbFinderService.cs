using Yact.Domain.Entities.Activity;
using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Climbs;

public interface IClimbFinderService
{
    List<ActivityClimb> FindClimbs(List<RecordData> records);
    List<string> GetDebugTrace();
}

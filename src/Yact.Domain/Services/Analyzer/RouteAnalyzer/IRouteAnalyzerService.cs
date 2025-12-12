using Yact.Domain.Entities.Activity;
using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer;

public interface IRouteAnalyzerService
{
    List<ActivityClimb> AnalyzeRoute(List<RecordData> records);
    void GetRouteDistances(List<RecordData> records);
    List<string> GetDebugTrace();
}

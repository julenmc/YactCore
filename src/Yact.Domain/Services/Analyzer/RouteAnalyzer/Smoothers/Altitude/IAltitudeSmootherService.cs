using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Smoothers.Altitude;

public interface IAltitudeSmootherService
{
    void Smooth(List<RecordData> records);
}

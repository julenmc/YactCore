using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.DistanceCalculator;

public interface IDistanceCalculator
{
    void CalculateDistances(List<RecordData> records);
    double CalculateDistanceFromToPoints(CoordinatesData from, CoordinatesData to);
}

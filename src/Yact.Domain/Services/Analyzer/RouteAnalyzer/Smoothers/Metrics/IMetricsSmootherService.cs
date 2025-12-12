namespace Yact.Domain.Services.Analyzer.RouteAnalyzer.Smoothers.Metrics;

public interface IMetricsSmootherService<T>
{
    T Smooth(List<float> records, int windowSize);
}

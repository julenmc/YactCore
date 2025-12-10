namespace Yact.Domain.Services.Smoothers.Metrics;

public interface IMetricsSmootherService<T>
{
    T Smooth(List<float> records, int windowSize);
}

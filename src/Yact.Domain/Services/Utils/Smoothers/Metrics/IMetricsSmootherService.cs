namespace Yact.Domain.Services.Utils.Smoothers.Metrics;

public interface IMetricsSmootherService<T>
{
    T Smooth(List<float> records, int windowSize);
}

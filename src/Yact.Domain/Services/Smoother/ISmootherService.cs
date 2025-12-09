using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Smoother;

public interface ISmootherService<T>
{
    T Smooth(List<float> records, int windowSize);
}

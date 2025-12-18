using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Services.Utils.Smoothers.Altitude;

public interface IAltitudeSmootherService
{
    void Smooth(List<RecordData> records);
}

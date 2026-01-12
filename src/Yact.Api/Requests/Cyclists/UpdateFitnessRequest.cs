using Yact.Application.Common;

namespace Yact.Api.Requests.Cyclists;

public record UpdateFitnessRequest (
    ushort? HeightCm,
    float? WeightKg,
    ushort? FtpWatts,
    float? Vo2Max,
    Dictionary<int, int>? PowerCurveBySeconds,
    Dictionary<string, Zone>? HrZones,
    Dictionary<string, Zone>? PowerZones);

using Yact.Application.ReadModels.Common;

namespace Yact.Api.Requests.Cyclists;

public record UpdateFitnessRequest (
    ushort? HeightCm,
    float? WeightKg,
    ushort? FtpWatts,
    float? Vo2Max,
    Dictionary<int, int>? PowerCurveBySeconds,
    Dictionary<string, ZoneReadModel>? HrZones,
    Dictionary<string, ZoneReadModel>? PowerZones);

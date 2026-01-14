using MediatR;
using Yact.Application.ReadModels.Common;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record UpdateFitnessCommand(
    Guid CyclistId,
    ushort? HeightCm,
    float? WeightKg,
    ushort? FtpWatts,
    float? Vo2Max,
    Dictionary<int, int>? PowerCurveBySeconds,
    Dictionary<string, ZoneReadModel>? HrZones,
    Dictionary<string, ZoneReadModel>? PowerZones) : IRequest<Guid>;

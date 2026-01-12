using MediatR;
using Yact.Application.Common;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record UpdateFitnessCommand(
    Guid CyclistId,
    ushort? HeightCm,
    float? WeightKg,
    ushort? FtpWatts,
    float? Vo2Max,
    Dictionary<int, int>? PowerCurveBySeconds,
    Dictionary<string, Zone>? HrZones,
    Dictionary<string, Zone>? PowerZones) : IRequest<Guid>;

using MediatR;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record CreateCyclistCommand(
    string Name,
    string LastName,
    DateTime BirthDate,
    ushort HeightCm,
    float WeightKg) : IRequest<Guid>;

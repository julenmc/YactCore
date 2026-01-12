using MediatR;

namespace Yact.Application.UseCases.Climbs.Commands;

public record UpdateClimbCommand(
    Guid Id,
    string Name) : IRequest<Guid>;
using MediatR;

namespace Yact.Application.UseCases.Climbs.Commands;

public record DeleteClimbByIdCommand(Guid Id) : IRequest<Guid>;

using MediatR;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record DeleteCyclistCommand(Guid Id) : IRequest<Guid>;

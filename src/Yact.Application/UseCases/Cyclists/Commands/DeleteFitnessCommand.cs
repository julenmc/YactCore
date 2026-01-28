using MediatR;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record DeleteFitnessCommand(Guid Id, Guid CyclistId) : IRequest<Guid>;

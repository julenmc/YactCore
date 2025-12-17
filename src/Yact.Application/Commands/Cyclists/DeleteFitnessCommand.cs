using MediatR;

namespace Yact.Application.Commands.Cyclists;

public record DeleteFitnessCommand(int Id) : IRequest<int?>;

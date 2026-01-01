using MediatR;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record DeleteFitnessCommand(int Id) : IRequest<int?>;

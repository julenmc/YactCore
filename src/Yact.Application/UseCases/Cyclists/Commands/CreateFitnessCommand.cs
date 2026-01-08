using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record CreateFitnessCommand(
    CyclistFitnessDto Fitness,
    Guid CyclistId) : IRequest<Guid>;

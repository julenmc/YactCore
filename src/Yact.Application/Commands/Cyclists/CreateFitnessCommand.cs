using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Cyclists;

public record CreateFitnessCommand(
    CyclistFitnessDto Fitness,
    int CyclistId) : IRequest<int>;

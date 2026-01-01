using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetCyclistLatestFitnessQuery (int Id) : IRequest<CyclistFitnessDto>;

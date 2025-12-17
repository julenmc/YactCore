using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Cyclists;

public record GetCyclistLatestFitnessQuery (int Id) : IRequest<CyclistFitnessDto>;

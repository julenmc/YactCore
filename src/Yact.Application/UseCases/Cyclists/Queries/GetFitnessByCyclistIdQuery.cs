using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetFitnessByCyclistIdQuery (int Id) : IRequest<IEnumerable<CyclistFitnessDto>>;

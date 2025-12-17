using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Cyclists;

public record GetFitnessByCyclistIdQuery (int Id) : IRequest<IEnumerable<CyclistFitnessDto>>;

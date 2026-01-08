using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetFitnessByCyclistIdQuery (Guid Id) : IRequest<IEnumerable<CyclistFitnessDto>>;

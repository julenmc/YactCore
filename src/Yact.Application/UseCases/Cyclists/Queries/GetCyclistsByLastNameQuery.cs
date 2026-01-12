using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Queries;

public record GetCyclistsByLastNameQuery (string LastName) : IRequest<IEnumerable<CyclistResponse>>;

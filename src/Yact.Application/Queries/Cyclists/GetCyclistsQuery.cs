using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Cyclists;

public record GetCyclistsQuery : IRequest<IEnumerable<CyclistDto>>;

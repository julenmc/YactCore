using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Climbs;

public record GetClimbsQuery : IRequest<IEnumerable<ClimbDto>>;

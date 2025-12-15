using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Climbs;

public record GetClimbByIdQuery(int Id) : IRequest<ClimbDto>;
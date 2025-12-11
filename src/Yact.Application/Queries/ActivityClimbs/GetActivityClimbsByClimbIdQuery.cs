using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.ActivityClimbs;

public record GetActivityClimbsByClimbIdQuery(int Id) : IRequest<List<ActivityClimbDto>>;

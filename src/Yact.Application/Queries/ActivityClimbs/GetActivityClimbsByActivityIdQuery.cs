using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.ActivityClimbs;

public record GetActivityClimbsByActivityIdQuery(int Id) : IRequest<List<ActivityClimbDto>>;

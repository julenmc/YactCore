using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.ActivityClimbs.Queries;

public record GetActivityClimbsByClimbIdQuery(Guid Id) : IRequest<List<ActivityClimbDto>>;

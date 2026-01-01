using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.ActivityClimbs.Queries;

public record GetActivityClimbsByClimbIdQuery(int Id) : IRequest<List<ActivityClimbDto>>;

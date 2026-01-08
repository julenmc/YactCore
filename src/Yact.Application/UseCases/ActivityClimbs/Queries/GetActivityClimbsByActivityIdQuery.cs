using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.ActivityClimbs.Queries;

public record GetActivityClimbsByActivityIdQuery(Guid Id) : IRequest<List<ActivityClimbDto>>;

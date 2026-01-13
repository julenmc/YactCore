using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Queries;

public record GetActivitiesByCyclisIdQuery(Guid Id) : IRequest<IEnumerable<ActivityResponse>>;

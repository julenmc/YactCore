using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Activities;

public record GetActivitiesByCyclisIdQuery(int Id) : IRequest<IEnumerable<ActivityInfoDto>>;

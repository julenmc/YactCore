using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Queries.Activities;

public record GetActivitiesQuery : IRequest<IEnumerable<ActivityInfoDto>>;

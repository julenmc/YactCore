using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Queries;

public record GetActivitiesQuery : IRequest<IEnumerable<ActivityDto>>;

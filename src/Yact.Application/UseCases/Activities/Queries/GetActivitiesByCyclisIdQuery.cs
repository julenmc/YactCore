using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Queries;

public record GetActivitiesByCyclisIdQuery(int Id) : IRequest<IEnumerable<ActivityDto>>;

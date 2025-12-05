using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Activities.GetActivities;

public record GetActivitiesQuery : IRequest<IEnumerable<ActivityDto>>;

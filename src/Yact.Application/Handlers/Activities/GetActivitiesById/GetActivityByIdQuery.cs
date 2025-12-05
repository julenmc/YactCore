using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Activities.GetActivitiesById;

public record GetActivityByIdQuery (int Id) : IRequest<ActivityDto>;

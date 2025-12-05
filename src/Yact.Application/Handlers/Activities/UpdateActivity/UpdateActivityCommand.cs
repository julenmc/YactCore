using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Activities.UpdateActivity;

public record UpdateActivityCommand (ActivityDto ActivityDto) : IRequest<ActivityDto>;

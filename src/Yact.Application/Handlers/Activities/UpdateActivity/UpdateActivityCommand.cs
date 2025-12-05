using MediatR;
using Yact.Application.DTOs;

namespace Yact.Application.Handlers.Activities.UpdateActivity;

public record UpdateActivityCommand (ActivityInfoDto ActivityDto) : IRequest<ActivityInfoDto>;

using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Activities;

public record UpdateActivityCommand (ActivityInfoDto ActivityDto) : IRequest<ActivityInfoDto>;

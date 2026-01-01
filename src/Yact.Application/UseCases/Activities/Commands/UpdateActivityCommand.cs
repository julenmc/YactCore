using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Commands;

public record UpdateActivityCommand (ActivityInfoDto ActivityDto) : IRequest<ActivityInfoDto>;

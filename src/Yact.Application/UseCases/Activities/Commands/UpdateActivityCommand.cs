using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Commands;

public record UpdateActivityCommand (ActivityDto ActivityDto) : IRequest<ActivityDto>;

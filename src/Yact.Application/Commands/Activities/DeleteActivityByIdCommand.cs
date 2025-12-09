using MediatR;

namespace Yact.Application.Commands.Activities;

public record DeleteActivityByIdCommand (int Id) : IRequest<int>;
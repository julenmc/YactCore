using MediatR;

namespace Yact.Application.Handlers.Activities.DeleteActivityById;

public record DeleteActivityByIdCommand (int Id) : IRequest<int>;
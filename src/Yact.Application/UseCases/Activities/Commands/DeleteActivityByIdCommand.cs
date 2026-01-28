using MediatR;

namespace Yact.Application.UseCases.Activities.Commands;

public record DeleteActivityByIdCommand (Guid Id) : IRequest<Guid>;
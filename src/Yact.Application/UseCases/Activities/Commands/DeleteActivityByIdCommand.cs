using MediatR;

namespace Yact.Application.UseCases.Activities.Commands;

public record DeleteActivityByIdCommand (int Id) : IRequest<int>;
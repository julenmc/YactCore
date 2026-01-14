using MediatR;

namespace Yact.Application.UseCases.Activities.Commands;

public record UpdateActivityCommand (
    Guid Id,
    string? Name,
    string? Description) : IRequest<Guid>;

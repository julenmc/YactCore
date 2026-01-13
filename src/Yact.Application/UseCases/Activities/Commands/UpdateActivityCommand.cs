using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Activities.Commands;

public record UpdateActivityCommand (
    Guid Id,
    string? Name,
    string? Description) : IRequest<ActivityResponse>;

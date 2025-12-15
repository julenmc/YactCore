using MediatR;

namespace Yact.Application.Commands.Climbs;

public record DeleteClimbByIdCommand(int Id) : IRequest<int>;

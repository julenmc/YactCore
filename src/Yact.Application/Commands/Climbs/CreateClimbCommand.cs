using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Climbs;

public record CreateClimbCommand(ClimbDto Climb) : IRequest<int>;

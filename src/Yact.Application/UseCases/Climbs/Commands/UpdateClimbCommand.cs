using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Climbs.Commands;

public record UpdateClimbCommand(ClimbDto Climb) : IRequest<int>;
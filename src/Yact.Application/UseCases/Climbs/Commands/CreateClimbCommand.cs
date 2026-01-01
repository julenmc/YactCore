using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Climbs.Commands;

public record CreateClimbCommand(ClimbDto Climb) : IRequest<int>;

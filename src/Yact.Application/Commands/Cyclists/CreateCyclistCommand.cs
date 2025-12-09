using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Cyclists;

public record CreateCyclistCommand(CyclistDto CyclistInfo) : IRequest<int>;

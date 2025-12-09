using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Cyclists;

public record UpdateCyclistCommand(CyclistDto Cyclist) : IRequest<CyclistDto>;

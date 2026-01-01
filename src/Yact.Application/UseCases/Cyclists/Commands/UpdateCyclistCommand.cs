using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record UpdateCyclistCommand(CyclistDto Cyclist) : IRequest<CyclistDto>;

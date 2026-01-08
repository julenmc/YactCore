using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record CreateCyclistCommand(CyclistDto CyclistInfo) : IRequest<Guid>;

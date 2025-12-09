using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.Commands.Cyclists;

public record DeleteCyclistCommand(int Id) : IRequest<int>;

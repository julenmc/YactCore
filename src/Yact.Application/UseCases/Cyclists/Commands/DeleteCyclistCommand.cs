using MediatR;
using Yact.Application.Responses;

namespace Yact.Application.UseCases.Cyclists.Commands;

public record DeleteCyclistCommand(int Id) : IRequest<int>;

using MediatR;

namespace Yact.Application.UseCases.Climbs.Commands;

public record DeleteClimbByIdCommand(int Id) : IRequest<int>;

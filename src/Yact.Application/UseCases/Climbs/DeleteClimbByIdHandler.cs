using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.UseCases.Climbs;

public class DeleteClimbByIdHandler : IRequestHandler<DeleteClimbByIdCommand, Guid>
{
    private readonly IClimbRepository _repository;

    public DeleteClimbByIdHandler(
        IClimbRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle (DeleteClimbByIdCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _repository.RemoveByIdAsync(ClimbId.From(command.Id));
        return deleted != null ? deleted.Id.Value : Guid.Empty;
    }
}

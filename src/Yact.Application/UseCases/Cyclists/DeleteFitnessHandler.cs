using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class DeleteFitnessHandler : IRequestHandler<DeleteFitnessCommand, Guid>
{
    private readonly ICyclistFitnessRepository _repository;

    public DeleteFitnessHandler(ICyclistFitnessRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle (DeleteFitnessCommand command, CancellationToken cancellationToken)
    {
        var obj = await _repository.DeleteFitnessAsync(CyclistFitnessId.From(command.Id));
        return obj?.Id.Value ?? Guid.Empty;
    }
}

using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class DeleteFitnessHandler : IRequestHandler<DeleteFitnessCommand, Guid>
{
    private readonly ICyclistRepository _repository;

    public DeleteFitnessHandler(ICyclistRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle (DeleteFitnessCommand command, CancellationToken cancellationToken)
    {
        var cyclist = await _repository.GetByIdAsync(CyclistId.From(command.CyclistId));
        if (cyclist == null)
            throw new CyclistNotFoundException(command.CyclistId);

        cyclist.RemoveFitness(CyclistFitnessId.From(command.Id));
        await _repository.UpdateAsync(cyclist);

        return command.Id;
    }
}

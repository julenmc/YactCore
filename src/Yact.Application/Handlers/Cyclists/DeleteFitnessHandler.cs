using MediatR;
using Yact.Application.Commands.Cyclists;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Cyclists;

public class DeleteFitnessHandler : IRequestHandler<DeleteFitnessCommand, int?>
{
    private readonly ICyclistFitnessRepository _repository;

    public DeleteFitnessHandler(ICyclistFitnessRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> Handle (DeleteFitnessCommand command, CancellationToken cancellationToken)
    {
        var obj = await _repository.DeleteFitnessAsync(command.Id);
        return obj?.Id;
    }
}

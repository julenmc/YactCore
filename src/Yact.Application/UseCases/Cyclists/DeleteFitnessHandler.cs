using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
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
        throw new NotImplementedException();
    }
}

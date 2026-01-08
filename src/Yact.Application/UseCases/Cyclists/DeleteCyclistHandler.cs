using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class DeleteCyclistHandler : IRequestHandler<DeleteCyclistCommand, Guid>
{
    private readonly ICyclistRepository _repository;

    public DeleteCyclistHandler(
        ICyclistRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(DeleteCyclistCommand request, CancellationToken cancelationToken)
    {
        var cyclist = await _repository.RemoveByIdAsync(CyclistId.From(request.Id));
        return cyclist?.Id.Value ?? Guid.Empty;
    }
}

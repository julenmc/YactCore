using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class DeleteClimbByIdHandler : IRequestHandler<DeleteClimbByIdCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public DeleteClimbByIdHandler(
        IMapper mapper,
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<int> Handle (DeleteClimbByIdCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _repository.RemoveByIdAsync(command.Id);
        return deleted != null ? deleted.Id : -1;
    }
}

using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Exceptions.Climbs;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Application.UseCases.Climbs;

public class UpdateClimbHandler : IRequestHandler<UpdateClimbCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public UpdateClimbHandler(
        IMapper mapper, 
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Guid> Handle (UpdateClimbCommand command, CancellationToken cancellationToken)
    {
        var climb = await _repository.GetByIdAsync(ClimbId.From(command.Id));
        if (climb == null)
            throw new ClimbNotFoundException(command.Id);

        var updated = await _repository.UpdateAsync(climb);
        return updated != null ? updated.Id.Value : Guid.Empty;
    }
}

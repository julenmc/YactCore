using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;

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
        var climb = _mapper.Map<Climb>(command.Climb);
        var updated = await _repository.UpdateAsync(climb);
        return updated != null ? updated.Id.Value : Guid.Empty;
    }
}

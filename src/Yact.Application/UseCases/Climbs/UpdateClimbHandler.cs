using AutoMapper;
using MediatR;
using Yact.Application.Commands.Climbs;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class UpdateClimbHandler : IRequestHandler<UpdateClimbCommand, int>
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

    public async Task<int> Handle (UpdateClimbCommand command, CancellationToken cancellationToken)
    {
        var climb = _mapper.Map<ClimbData>(command.Climb);
        var updated = await _repository.UpdateAsync(climb);
        return updated != null ? updated.Id : -1;
    }
}

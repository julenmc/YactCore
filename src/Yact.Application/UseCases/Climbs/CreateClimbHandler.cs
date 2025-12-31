using AutoMapper;
using MediatR;
using Yact.Application.Commands.Climbs;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class CreateClimbHandler : IRequestHandler<CreateClimbCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IClimbRepository _repository;

    public CreateClimbHandler(
        IMapper mapper, 
        IClimbRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<int> Handle (CreateClimbCommand command, CancellationToken cancellationToken)
    {
        var climb = _mapper.Map<ClimbData>(command.Climb);
        var added = await _repository.AddAsync(climb);
        return added.Id;
    }
}

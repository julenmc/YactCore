using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Climbs.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Climbs;

public class CreateClimbHandler : IRequestHandler<CreateClimbCommand, Guid>
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

    public async Task<Guid> Handle (CreateClimbCommand command, CancellationToken cancellationToken)
    {
        var climb = _mapper.Map<Climb>(command.Climb);
        var added = await _repository.AddAsync(climb);
        return added.Id.Value;
    }
}

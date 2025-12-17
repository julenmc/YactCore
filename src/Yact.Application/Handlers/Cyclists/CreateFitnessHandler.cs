using AutoMapper;
using MediatR;
using Yact.Application.Commands.Cyclists;
using Yact.Domain.Entities.Cyclist;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Cyclists;

public class CreateFitnessHandler : IRequestHandler<CreateFitnessCommand, int>
{
    private readonly IMapper _mapper;
    private readonly ICyclistFitnessRepository _repository;

    public CreateFitnessHandler(
        IMapper mapper,
        ICyclistFitnessRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<int> Handle (CreateFitnessCommand command, CancellationToken cancellationToken)
    {
        var fitness = _mapper.Map<CyclistFitness>(command.Fitness);
        var obj = await _repository.AddFitnessAsync(fitness, command.CyclistId);
        return obj.Id;
    }
}

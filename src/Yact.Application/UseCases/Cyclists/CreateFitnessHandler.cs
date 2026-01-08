using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class CreateFitnessHandler : IRequestHandler<CreateFitnessCommand, Guid>
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

    public async Task<Guid> Handle (CreateFitnessCommand command, CancellationToken cancellationToken)
    {
        var fitness = _mapper.Map<CyclistFitness>(command.Fitness);
        var obj = await _repository.AddFitnessAsync(fitness, CyclistId.From(command.CyclistId));
        return obj.Id.Value;
    }
}

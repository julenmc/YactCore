using AutoMapper;
using MediatR;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class UpdateFitness : IRequestHandler<UpdateFitnessCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly ICyclistRepository _repository;

    public UpdateFitness(
        IMapper mapper,
        ICyclistRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Guid> Handle (UpdateFitnessCommand command, CancellationToken cancellationToken)
    {
        var cyclist = await _repository.GetByIdAsync(CyclistId.From(command.CyclistId));
        if (cyclist == null)
            throw new NoCyclistException(command.CyclistId);

        var fitnessId = cyclist.UpdateFitness(
            (command.HeightCm != null && command.WeightKg != null) ? 
                new Size((ushort)command.HeightCm, (float)command.WeightKg) : null,
            command.FtpWatts,
            command.Vo2Max,
            command.PowerCurveBySeconds,
            command.HrZones != null ?
                command.HrZones.ToDictionary(kvp => int.Parse(kvp.Key), kvp => _mapper.Map<Zone>(kvp.Value)) : null);

        await _repository.UpdateAsync(cyclist);
        return fitnessId.Value;
    }
}

using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Cyclists;

public class CreateCyclistHandler : IRequestHandler<CreateCyclistCommand, Guid>
{
    private readonly ICyclistRepository _repository;
    private readonly IMapper _mapper;

    public CreateCyclistHandler(
        ICyclistRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateCyclistCommand command, CancellationToken cancellationToken)
    {
        var cyclistGuid = Guid.NewGuid();
        var cyclist = Cyclist.Create(
            CyclistId.From(cyclistGuid),
            command.Name,
            command.LastName,
            command.BirthDate,
            new Size(command.HeightCm, command.WeightKg));

        var saved = await _repository.AddAsync(cyclist);

        return saved?.Id.Value ?? Guid.Empty;
    }
}

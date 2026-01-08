using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Commands;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;

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

    public async Task<Guid> Handle(CreateCyclistCommand request, CancellationToken cancellationToken)
    {
        var cyclist = _mapper.Map<Cyclist>(request.CyclistInfo);
        var saved = await _repository.AddAsync(cyclist);

        return saved?.Id.Value ?? Guid.Empty;
    }
}

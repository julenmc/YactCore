using AutoMapper;
using MediatR;
using Yact.Application.Commands.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Entities.Cyclist;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class CreateCyclistHandler : IRequestHandler<CreateCyclistCommand, int>
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

    public async Task<int> Handle(CreateCyclistCommand request, CancellationToken cancellationToken)
    {
        var cyclist = _mapper.Map<Cyclist>(request.CyclistInfo);
        var saved = await _repository.AddAsync(cyclist);

        return _mapper.Map<CyclistDto>(saved).Id;
    }
}

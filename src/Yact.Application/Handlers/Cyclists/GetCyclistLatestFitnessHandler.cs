using AutoMapper;
using MediatR;
using Yact.Application.Queries.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Cyclists;

public class GetCyclistLatestFitnessHandler : IRequestHandler<GetCyclistLatestFitnessQuery, CyclistFitnessDto>
{
    private readonly IMapper _mapper;
    private readonly ICyclistFitnessRepository _repository;

    public GetCyclistLatestFitnessHandler(
        IMapper mapper,
        ICyclistFitnessRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<CyclistFitnessDto> Handle (GetCyclistLatestFitnessQuery query, CancellationToken cancellationToken)
    {
        var obj = await _repository.GetCyclistLatestFitnessAsync(query.Id);
        return _mapper.Map<CyclistFitnessDto>(obj);
    }
}

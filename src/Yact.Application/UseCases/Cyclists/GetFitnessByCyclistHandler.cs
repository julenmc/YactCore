using AutoMapper;
using MediatR;
using Yact.Application.Queries.Cyclists;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Cyclists;

public class GetFitnessByCyclistHandler : IRequestHandler<GetFitnessByCyclistIdQuery, IEnumerable<CyclistFitnessDto>>
{
    private readonly IMapper _mapper;
    private readonly ICyclistFitnessRepository _repository;

    public GetFitnessByCyclistHandler(
        IMapper mapper,
        ICyclistFitnessRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<IEnumerable<CyclistFitnessDto>> Handle (GetFitnessByCyclistIdQuery query, CancellationToken cancellationToken)
    {
        var objList = await _repository.GetFitnessByCyclistIdAsync(query.Id);
        return _mapper.Map<IEnumerable<CyclistFitnessDto>>(objList);
    }
}

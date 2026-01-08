using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Cyclists.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

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
        var objList = await _repository.GetFitnessByCyclistIdAsync(CyclistId.From(query.Id));
        return _mapper.Map<IEnumerable<CyclistFitnessDto>>(objList);
    }
}

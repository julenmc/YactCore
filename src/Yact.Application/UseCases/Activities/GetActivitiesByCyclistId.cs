using AutoMapper;
using MediatR;
using Yact.Application.Mapping;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Activities;

public class GetActivitiesByCyclistId : IRequestHandler<GetActivitiesByCyclisIdQuery, IEnumerable<ActivityResponse>>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivitiesByCyclistId(
        IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActivityResponse>> Handle (GetActivitiesByCyclisIdQuery query, CancellationToken cancellationToken)
    {
        var activityList = await _repository.GetByCyclistIdAsync(CyclistId.From(query.Id));
        return _mapper.Map<IEnumerable<ActivityResponse>>(activityList);
    }
}

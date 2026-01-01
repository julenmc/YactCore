using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Activities;

public class GetActivitiesByCyclistId : IRequestHandler<GetActivitiesByCyclisIdQuery, IEnumerable<ActivityInfoDto>>
{
    private readonly IMapper _mapper;
    private readonly IActivityRepository _repository;

    public GetActivitiesByCyclistId(
        IMapper mapper,
        IActivityRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<IEnumerable<ActivityInfoDto>> Handle (GetActivitiesByCyclisIdQuery query, CancellationToken cancellationToken)
    {
        var activityList = await _repository.GetByCyclistIdAsync(query.Id);
        return _mapper.Map<IEnumerable<ActivityInfoDto>>(activityList); 
    }
}

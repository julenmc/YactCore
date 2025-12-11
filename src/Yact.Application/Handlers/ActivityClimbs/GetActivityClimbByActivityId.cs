using AutoMapper;
using MediatR;
using Yact.Application.Queries.ActivityClimbs;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.ActivityClimbs;

public class GetActivityClimbByActivityId : IRequestHandler<GetActivityClimbsByActivityIdQuery, List<ActivityClimbDto>>
{
    private readonly IActivityClimbRepository _repository;
    private readonly IMapper _mapper;

    public GetActivityClimbByActivityId(
        IActivityClimbRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ActivityClimbDto>> Handle(GetActivityClimbsByActivityIdQuery query, CancellationToken cancellationToken)
    {
        var activityClimbs = await _repository.GetByActivityAsync(query.Id);
        return _mapper.Map<List<ActivityClimbDto>>(activityClimbs);
    }
}

using AutoMapper;
using MediatR;
using Yact.Application.Queries.ActivityClimbs;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.ActivityClimbs;

public class GetActivityClimbByClimbId : IRequestHandler<GetActivityClimbsByClimbIdQuery, List<ActivityClimbDto>>
{
    private readonly IActivityClimbRepository _repository;
    private readonly IMapper _mapper;

    public GetActivityClimbByClimbId(
        IActivityClimbRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ActivityClimbDto>> Handle (GetActivityClimbsByClimbIdQuery query, CancellationToken cancellationToken)
    {
        var activityClimbs = await _repository.GetByClimbAsync(query.Id);
        return _mapper.Map<List<ActivityClimbDto>>(activityClimbs);
    }
}

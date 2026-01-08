using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Yact.Application.Responses;
using Yact.Application.UseCases.ActivityClimbs.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.ActivityClimbs;

public class GetActivityClimbByActivityId : IRequestHandler<GetActivityClimbsByActivityIdQuery, List<ActivityClimbDto>>
{
    private readonly IActivityClimbRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetActivityClimbByActivityId> _logger;

    public GetActivityClimbByActivityId(
        IActivityClimbRepository repository,
        IMapper mapper,
        ILogger<GetActivityClimbByActivityId> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ActivityClimbDto>> Handle(GetActivityClimbsByActivityIdQuery query, CancellationToken cancellationToken)
    {
        var activityClimbs = await _repository.GetByActivityAsync(ActivityId.From(query.Id));
        return _mapper.Map<List<ActivityClimbDto>>(activityClimbs);
    }
}

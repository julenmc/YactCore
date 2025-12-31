using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Yact.Application.Queries.ActivityClimbs;
using Yact.Application.Responses;
using Yact.Domain.Repositories;

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
        var activityClimbs = await _repository.GetByActivityAsync(query.Id);

        // If there's at least one unknown climb, the activity should be read to get the climbs metrics
        if (activityClimbs.Any(a => a.Data.Name == "Unknown" || a.ClimbId == 1))
        {
            _logger.LogDebug($"There's an unknown climb in the activity with ID {query.Id}");
        }

        return _mapper.Map<List<ActivityClimbDto>>(activityClimbs);
    }
}

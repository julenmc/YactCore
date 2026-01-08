using AutoMapper;
using MediatR;
using Yact.Application.Mapping;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Activities;

public class GetActivities : IRequestHandler<GetActivitiesQuery, IEnumerable<ActivityDto>>
{
    private readonly IActivityRepository _repository;

    public GetActivities(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ActivityDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = await _repository.GetAllAsync();
        return activities
            .Select(a => a.ToModel())
            .ToList();
    }
}

using AutoMapper;
using MediatR;
using Yact.Application.Mapping;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Application.UseCases.Activities;

public class GetActivitiesByCyclistId : IRequestHandler<GetActivitiesByCyclisIdQuery, IEnumerable<ActivityDto>>
{
    private readonly IActivityRepository _repository;

    public GetActivitiesByCyclistId(
        IActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ActivityDto>> Handle (GetActivitiesByCyclisIdQuery query, CancellationToken cancellationToken)
    {
        var activityList = await _repository.GetByCyclistIdAsync(CyclistId.From(query.Id));
        return activityList
            .Select(a => a.ToModel())
            .ToList();
    }
}

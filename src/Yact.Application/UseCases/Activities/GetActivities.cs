using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Activities;

public class GetActivities : IRequestHandler<GetActivitiesQuery, IEnumerable<ActivityInfoDto>>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivities(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActivityInfoDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ActivityInfoDto>>(activities);
    }
}

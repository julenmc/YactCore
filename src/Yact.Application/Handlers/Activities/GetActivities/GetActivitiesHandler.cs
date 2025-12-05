using AutoMapper;
using MediatR;
using Yact.Application.DTOs;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Activities.GetActivities;

public class GetActivitiesHandler : IRequestHandler<GetActivitiesQuery, IEnumerable<ActivityDto>>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivitiesHandler(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ActivityDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
    {
        var activities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ActivityDto>>(activities);
    }
}

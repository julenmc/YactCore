using AutoMapper;
using MediatR;
using Yact.Application.Mapping;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class GetActivityById : IRequestHandler<GetActivityByIdQuery, ActivityDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivityById(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityDto> Handle(GetActivityByIdQuery request, CancellationToken cancellation)
    {
        var activity = await _repository.GetByIdAsync(ActivityId.From(request.Id));
        if (activity == null)
        {
            throw new ActivityNotFoundException(request.Id);
        }
        return activity.ToModel();
    }
}

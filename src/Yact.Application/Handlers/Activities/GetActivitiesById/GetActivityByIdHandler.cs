using AutoMapper;
using MediatR;
using Yact.Application.DTOs;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Activities.GetActivitiesById;

public class GetActivityByIdHandler : IRequestHandler<GetActivityByIdQuery, ActivityDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivityByIdHandler(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityDto> Handle(GetActivityByIdQuery request, CancellationToken cancellation)
    {
        var activity = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ActivityDto>(activity);
    }
}

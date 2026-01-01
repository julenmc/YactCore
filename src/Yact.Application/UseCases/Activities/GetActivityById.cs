using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Queries;
using Yact.Domain.Repositories;

namespace Yact.Application.UseCases.Activities;

public class GetActivityById : IRequestHandler<GetActivityByIdQuery, ActivityInfoDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public GetActivityById(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityInfoDto> Handle(GetActivityByIdQuery request, CancellationToken cancellation)
    {
        var activity = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ActivityInfoDto>(activity);
    }
}

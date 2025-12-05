using AutoMapper;
using MediatR;
using Yact.Application.DTOs;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;

namespace Yact.Application.Handlers.Activities.UpdateActivity;

public class UpdateActivityHandler : IRequestHandler<UpdateActivityCommand, ActivityInfoDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateActivityHandler(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityInfoDto> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = _mapper.Map<ActivityInfo>(command.ActivityDto);
        activity.UpdateDate = DateTime.Now;
        var updated = await _repository.UpdateAsync(activity);
        return _mapper.Map<ActivityInfoDto>(updated);
    }
}

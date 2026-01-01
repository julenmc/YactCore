using AutoMapper;
using MediatR;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class UpdateActivity : IRequestHandler<UpdateActivityCommand, ActivityInfoDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateActivity(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityInfoDto> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = _mapper.Map<Activity>(command.ActivityDto); // TODO: update time
        var updated = await _repository.UpdateAsync(activity);
        return _mapper.Map<ActivityInfoDto>(updated);
    }
}

using AutoMapper;
using MediatR;
using Yact.Application.Mapping;
using Yact.Application.Responses;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class UpdateActivity : IRequestHandler<UpdateActivityCommand, ActivityDto>
{
    private readonly IActivityRepository _repository;
    private readonly IMapper _mapper;

    public UpdateActivity(IActivityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ActivityDto> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = command.ActivityDto.ToDomain(); // TODO: update time
        var updated = await _repository.UpdateAsync(activity);
        if (updated == null)
        {
            throw new ActivityNotFoundException(command.ActivityDto.Id);
        }
        return updated.ToModel();
    }
}

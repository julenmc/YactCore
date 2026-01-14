using MediatR;
using Yact.Application.UseCases.Activities.Commands;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Application.UseCases.Activities;

public class UpdateActivity : IRequestHandler<UpdateActivityCommand, Guid>
{
    private readonly IActivityRepository _repository;

    public UpdateActivity(IActivityRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = await _repository.GetByIdAsync(ActivityId.From(command.Id));
        if (activity == null) 
            throw new ActivityNotFoundException(command.Id);

        if (command.Name != null) 
            activity.UpdateName(command.Name);
        if (command.Description != null)
            activity.UpdateDescription(command.Description);

        await _repository.UpdateAsync(activity);
        return activity.Id.Value;
    }
}

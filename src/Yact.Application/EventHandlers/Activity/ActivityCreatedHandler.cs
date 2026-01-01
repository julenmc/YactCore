using MediatR;
using Yact.Application.Common;
using Yact.Application.Services.Activities;
using Yact.Domain.Events.Activity;
using Yact.Domain.Exceptions.Activity;

namespace Yact.Application.EventHandlers.Activity;

public class ActivityCreatedHandler
    : INotificationHandler<DomainEventNotification<ActivityCreatedEvent>>
{
    private readonly FullReadActivityService _reader;
    private readonly ClimbHandlerService _climbHandler;

    public ActivityCreatedHandler(
        FullReadActivityService reader,
        ClimbHandlerService climbHandler)
    {
        _reader = reader;
        _climbHandler = climbHandler;
    }

    public async Task Handle(DomainEventNotification<ActivityCreatedEvent> activityCreatedEvent, CancellationToken cancellationToken)
    {
        var activity = await _reader.Execute(activityCreatedEvent.DomainEvent.ActivityId.Value);
        if (activity.Records == null || activity.Records.Values.Count == 0)
        {
            throw new NoDataException();
        }

        await _climbHandler.Execute(activity);
    }
}

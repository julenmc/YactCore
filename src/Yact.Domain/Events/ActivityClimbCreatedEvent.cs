using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Activity.ActivityClimb;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Events;

public record ActivityClimbCreatedEvent (
    ActivityClimbId activityClimbId,
    ClimbId ClimbId,
    ActivityId ActivityId
    ) : IDomainEvent;
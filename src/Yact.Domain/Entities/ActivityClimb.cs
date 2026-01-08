using Yact.Domain.Events;
using Yact.Domain.Primitives;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.ActivityClimb;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Entities;

public class ActivityClimb : AggregateRoot<ActivityClimbId>
{
    public ActivityId ActivityId { get; init; }
    public ClimbId ClimbId { get; init; }
    //public int IntervalId { get; set; }
    public double StartPointMeters { get; init; }

    private ActivityClimb(
        ActivityClimbId id,
        ActivityId activityId,
        ClimbId climbId,
        double start)
        :base (id)
    {
        ActivityId = activityId;
        ClimbId = climbId;
        StartPointMeters = start;
    }

    public static ActivityClimb Create(
        ActivityClimbId id,
        ActivityId activityId,
        ClimbId climbId,
        double start)
    {
        var activityClimb = Load(
            id, activityId, climbId, start);

        // Add event
        activityClimb.AddDomainEvent(new ActivityClimbCreatedEvent(
            id,
            climbId, 
            activityId));

        return activityClimb;
    }

    public static ActivityClimb Load(
        ActivityClimbId id,
        ActivityId activityId,
        ClimbId climbId,
        double start)
    {
        if (start < 0)
        {
            throw new ArgumentException("Start point cannot be less than zero", nameof(start));
        }
        return new ActivityClimb(
            id, activityId, climbId, start);
    }
}
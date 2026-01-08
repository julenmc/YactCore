using Yact.Infrastructure.Persistence.Models;
using Entities = Yact.Domain.Entities;
using VO = Yact.Domain.ValueObjects;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityClimbMapper
{
    internal static Entities.ActivityClimb ToDomain(this ActivityClimb model)
    {
        return Entities.ActivityClimb.Load(
            id: VO.ActivityClimb.ActivityClimbId.From(model.Id),
            activityId: VO.Activity.ActivityId.From(model.ActivityId),
            climbId: VO.Climb.ClimbId.From(model.ClimbId),
            start: model.StartPointMeters
        );
    }

    internal static ActivityClimb ToModel(this Domain.Entities.ActivityClimb entity)
    {
        return new ActivityClimb
        {
            Id = entity.Id.Value,
            ActivityId = entity.ActivityId.Value,
            ClimbId = entity.ClimbId.Value,
            StartPointMeters = entity.StartPointMeters
        };
    }
}

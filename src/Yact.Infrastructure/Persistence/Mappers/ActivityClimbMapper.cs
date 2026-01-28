using Yact.Application.ReadModels.ActivityClimbs;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityClimbMapper
{
    internal static ActivityClimbFromActivityReadModel ToModel(this ActivityClimbReadModel activityClimb)
    {
        return new ActivityClimbFromActivityReadModel
        {
            Id = activityClimb.Id,
            StartPointMeters = activityClimb.StartPointMeters,
            ClimbName = activityClimb.Climb?.Name ?? string.Empty,
        };
    }
}

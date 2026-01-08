using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Repositories;

public interface IActivityClimbRepository
{
    Task<List<ActivityClimb>> GetByActivityAsync(ActivityId activityId);
    Task<List<ActivityClimb>> GetByClimbAsync(ClimbId climbId);
    Task AddAsync(ActivityClimb activityClimb);
}

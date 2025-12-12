using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Repositories;

public interface IActivityClimbRepository
{
    Task<List<ActivityClimb>> GetByActivityAsync(int activityId);
    Task<List<ActivityClimb>> GetByClimbAsync(int climbId);
    Task AddAsync(ActivityClimb activityClimb, int activityId);
    Task AddAsync(IEnumerable<ActivityClimb> activityClimbs, int activityId);
}

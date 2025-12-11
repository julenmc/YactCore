using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Repositories;

public interface IActivityClimbRepository
{
    Task<List<ActivityClimb>> GetByActivityAsync(int activityId);
    Task<List<ActivityClimb>> GetByClimbAsync(int climbId);
}

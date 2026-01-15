using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.ActivityClimb;

namespace Yact.Domain.Repositories;

public interface IActivityClimbRepository
{
    Task<ActivityClimb?> GetById(ActivityClimbId id);
    Task<ActivityClimb> AddAsync(ActivityClimb activityClimb);
    Task<ActivityClimb?> RemoveAsync(ActivityClimbId id);
}

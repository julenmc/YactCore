using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(ActivityId id);
    Task<Activity> AddAsync(Activity activity);
    Task<Activity?> RemoveByIdAsync(ActivityId id);
    Task<Activity?> UpdateAsync(Activity activity);
}

using Yact.Domain.Entities.Activity;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<Activity?> GetByIdAsync(int id);
    Task<IEnumerable<Activity>> GetByCyclistIdAsync(int id);
    Task<Activity> AddAsync(ActivitySummary activity, string path, int cyclistId);
    Task<Activity?> RemoveByIdAsync(int id);
    Task<Activity?> UpdateAsync(Activity activity);
}

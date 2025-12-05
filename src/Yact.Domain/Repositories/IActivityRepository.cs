using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<Activity?> GetByIdAsync(int id);
    Task AddAsync(Activity activity);
    Task<Activity?> RemoveByIdAsync(int id);
    Task<Activity?> UpdateAsync(Activity activity);
}

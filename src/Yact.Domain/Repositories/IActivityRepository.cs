using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<ActivityInfo>> GetAllAsync();
    Task<ActivityInfo?> GetByIdAsync(int id);
    Task AddAsync(ActivityInfo activity, int cyclistId);
    Task<ActivityInfo?> RemoveByIdAsync(int id);
    Task<ActivityInfo?> UpdateAsync(ActivityInfo activity);
}

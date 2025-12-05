using Yact.Domain.Entities.Activity;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<ActivityInfo>> GetAllAsync();
    Task<ActivityInfo?> GetByIdAsync(int id);
    Task AddAsync(ActivityInfo activity);
    Task<ActivityInfo?> RemoveByIdAsync(int id);
    Task<ActivityInfo?> UpdateAsync(ActivityInfo activity);
}

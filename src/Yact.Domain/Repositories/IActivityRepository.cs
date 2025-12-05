using Yact.Domain.Models;

namespace Yact.Domain.Repositories;

public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetAllAsync();
    Task<Activity?> GetByIdAsync(int id);
    Task AddAsync(Activity activity);
}

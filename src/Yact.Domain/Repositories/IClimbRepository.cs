using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Repositories;

public interface IClimbRepository
{
    Task<IEnumerable<ClimbData>> GetAllAsync();
    Task<ClimbData?> GetByIdAsync(int id);
    Task AddAsync(ClimbData climb);
    Task<ClimbData?> RemoveByIdAsync(int id);
    Task<ClimbData?> UpdateAsync(ClimbData climb);
}

using Yact.Domain.Entities.Cyclist;

namespace Yact.Domain.Repositories;

public interface ICyclistRepository
{
    Task<IEnumerable<Cyclist>> GetAllAsync();
    Task<Cyclist?> GetByIdAsync(int id);
    Task<Cyclist?> AddAsync(Cyclist activity);
    Task<Cyclist?> RemoveByIdAsync(int id);
    Task<Cyclist?> UpdateAsync(Cyclist activity);
}

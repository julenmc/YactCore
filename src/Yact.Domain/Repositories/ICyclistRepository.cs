using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Repositories;

public interface ICyclistRepository
{
    Task<IEnumerable<Cyclist>> GetAllAsync();
    Task<Cyclist?> GetByIdAsync(CyclistId id);
    Task<Cyclist?> AddAsync(Cyclist activity);
    Task<Cyclist?> RemoveByIdAsync(CyclistId id);
    Task<Cyclist?> UpdateAsync(Cyclist activity);
}

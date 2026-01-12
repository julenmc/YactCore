using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Repositories;

public interface ICyclistRepository
{
    Task<IEnumerable<Cyclist>> GetAllAsync();
    Task<Cyclist?> GetByIdAsync(CyclistId id);
    Task<IEnumerable<Cyclist>> GetByLastName(string name);
    Task<Cyclist?> AddAsync(Cyclist activity);
    Task<Cyclist?> RemoveByIdAsync(CyclistId id);
    Task<Cyclist?> UpdateAsync(Cyclist activity);
    //Task<CyclistFitness> AddFitnessAsync(CyclistFitness fitness);
    //Task<Guid> DeleteFitnessAsync(CyclistFitnessId id);
}

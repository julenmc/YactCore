using Yact.Domain.Entities.Cyclist;

namespace Yact.Domain.Repositories;

public interface ICyclistFitnessRepository
{
    Task<IEnumerable<CyclistFitness>> GetFitnessByCyclistIdAsync(int cyclistId);
    Task<CyclistFitness?> GetCyclistLatestFitnessAsync(int cyclistId);
    Task<CyclistFitness> AddFitnessAsync(CyclistFitness fitness, int cyclistId);
    Task<CyclistFitness?> DeleteFitnessAsync(int id);
}

using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Domain.Repositories;

public interface ICyclistFitnessRepository
{
    Task<IEnumerable<CyclistFitness>> GetFitnessByCyclistIdAsync(CyclistId cyclistId);
    Task<CyclistFitness?> GetCyclistLatestFitnessAsync(CyclistId cyclistId);
    Task<CyclistFitness> AddFitnessAsync(CyclistFitness fitness, CyclistId cyclistId);
    Task<CyclistFitness?> DeleteFitnessAsync(CyclistFitnessId id);
}

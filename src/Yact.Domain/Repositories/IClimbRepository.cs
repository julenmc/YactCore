using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Domain.Repositories;

public interface IClimbRepository
{
    Task<Climb?> GetByIdAsync(ClimbId id);
    Task<IEnumerable<Climb>> GetByCoordinatesAsync(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax);
    Task<Climb> AddAsync(Climb climb);
    Task<Climb?> RemoveByIdAsync(ClimbId id);
    Task<Climb?> UpdateAsync(Climb climb);
}

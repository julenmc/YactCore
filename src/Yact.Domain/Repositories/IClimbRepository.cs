using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Repositories;

public interface IClimbRepository
{
    Task<IEnumerable<ClimbData>> GetAllAsync();
    Task<ClimbData?> GetByIdAsync(int id);
    Task<IEnumerable<ClimbData?>> GetByCoordinates(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax);
    Task<int> AddAsync(ClimbData climb);
    Task<int> RemoveByIdAsync(int id);
    Task<int> UpdateAsync(ClimbData climb);
}

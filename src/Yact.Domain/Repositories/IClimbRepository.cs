using Yact.Domain.Entities.Climb;

namespace Yact.Domain.Repositories;

public interface IClimbRepository
{
    Task<IEnumerable<ClimbData>> GetAllAsync();
    Task<ClimbData?> GetByIdAsync(int id);
    Task<IEnumerable<ClimbData>> GetByCoordinatesAsync(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax);
    Task<ClimbData> AddAsync(ClimbData climb);
    Task<ClimbData?> RemoveByIdAsync(int id);
    Task<ClimbData?> UpdateAsync(ClimbData climb);
}

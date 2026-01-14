using Yact.Application.ReadModels.Climbs;

namespace Yact.Application.Interfaces.Queries;

public interface IClimbQueries
{
    Task<IEnumerable<ClimbBasicReadModel>> GetByNameAsync(string name);
    Task<ClimbAdvancedReadModel> GetByIdAsync(Guid id);
}

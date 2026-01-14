using Yact.Application.ReadModels.Cyclists;

namespace Yact.Application.Interfaces.Repositories;

public interface ICyclistQueries
{
    Task<IEnumerable<CyclistBasicReadModel>> GetByLastNameAsync(string lastName);
    Task<CyclistAdvancedReadModel> GetByIdAsync(Guid id);
}

using Yact.Application.ReadModels.Activities;

namespace Yact.Application.Interfaces.Repositories;

public interface IActivityQueries
{
    Task<ActivityAdvancedReadModel> GetByIdAsync(Guid id);
}

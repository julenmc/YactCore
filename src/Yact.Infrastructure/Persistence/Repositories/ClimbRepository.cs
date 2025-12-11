using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ClimbRepository : IClimbRepository
{
    public async Task<IEnumerable<ClimbData>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<ClimbData?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task AddAsync(ClimbData climb)
    {
        throw new NotImplementedException();
    }

    public async Task<ClimbData?> RemoveByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ClimbData?> UpdateAsync(ClimbData climb)
    {
        throw new NotImplementedException();
    }

}

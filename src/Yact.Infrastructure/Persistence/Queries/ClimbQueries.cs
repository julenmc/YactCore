using Microsoft.EntityFrameworkCore;
using Yact.Application.Interfaces.Queries;
using Yact.Application.ReadModels.Climbs;
using Yact.Domain.Exceptions.Climbs;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;

namespace Yact.Infrastructure.Persistence.Queries;

public class ClimbQueries : IClimbQueries
{
    private readonly ReadDbContext _db;
    public ClimbQueries(ReadDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ClimbBasicReadModel>> GetByNameAsync(string name)
    {
        var climbs = await _db.Climbs
            .Where(c => c.Name == name)
            .ToListAsync();

        return climbs.Select(c => c.ToBasicModel());
    }

    public async Task<ClimbAdvancedReadModel> GetByIdAsync(Guid id)
    {
        var climb = await _db.Climbs
            .Where(c => c.Id == id)
            // TODO: Add activity climbs
            .FirstOrDefaultAsync();

        if (climb == null)
            throw new ClimbNotFoundException(id);

        return climb.ToAdvancedModel();
    }
}

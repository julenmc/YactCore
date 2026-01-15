using Microsoft.EntityFrameworkCore;
using Yact.Application.Interfaces.Repositories;
using Yact.Application.ReadModels.Activities;
using Yact.Domain.Exceptions.Activity;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;

namespace Yact.Infrastructure.Persistence.Queries;

public class ActivityQueries : IActivityQueries
{
    private readonly ReadDbContext _db;

    public ActivityQueries(ReadDbContext db)
    {
        _db = db;
    }

    public async Task<ActivityAdvancedReadModel> GetByIdAsync(Guid id)
    {
        var activity = await _db.Activities
            .Where(a => a.Id == id)
            .Include(a => a.ActivityClimbs)
            .FirstOrDefaultAsync();
        if (activity == null)
            throw new ActivityNotFoundException(id);

        return activity.ToAdvancedModel();
    }
}

using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.ActivityClimb;
using Yact.Infrastructure.Persistence.Data;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityClimbRepository : IActivityClimbRepository
{
    private readonly WriteDbContext _db;

    public ActivityClimbRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<ActivityClimb?> GetById(ActivityClimbId id)
    {
        return await _db.ActivityClimbs.FindAsync(id);
    }

    public async Task<ActivityClimb> AddAsync(ActivityClimb activityClimb)
    {
        var entry = await _db.ActivityClimbs.AddAsync(activityClimb);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<ActivityClimb?> RemoveAsync(ActivityClimbId id)
    {
        try
        {
            var activityClimb = await _db.ActivityClimbs.FindAsync(id);
            if (activityClimb == null)
                throw new ActivityNotFoundException(id.Value);

            _db.ActivityClimbs.Remove(activityClimb);
            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? activityClimb : null;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }
}

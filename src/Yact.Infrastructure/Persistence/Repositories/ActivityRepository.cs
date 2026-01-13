using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities;
using Yact.Domain.Exceptions.Activity;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Data;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Activity?> GetByIdAsync(ActivityId id)
    {
        return await _db.Activities.FindAsync(id);
    }

    public async Task<IEnumerable<Activity>> GetByCyclistIdAsync(CyclistId id)
    {
        return await _db.Activities
            .Where(a => a.CyclistId == id)
            .ToListAsync();
    }

    public async Task<Activity> AddAsync(Activity activity)
    {
        var entry = await _db.Activities.AddAsync(activity);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Activity?> RemoveByIdAsync(ActivityId id)
    {
        try
        {
            var activity = await _db.Activities.FindAsync(id);
            if (activity == null)
                throw new ActivityNotFoundException(id.Value);

            _db.Activities.Remove(activity);
            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? activity : null;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }

    public async Task<Activity?> UpdateAsync(Activity activity)
    {
        var updated = _db.Update(activity);
        if (updated == null) 
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity : null;
    }
}

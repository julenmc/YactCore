using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Data;

namespace Yact.Infrastructure.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ActivityInfo>> GetAllAsync()
    {
        return await _db.Activities.ToListAsync();
    }

    public async Task<ActivityInfo?> GetByIdAsync(int id)
    {
        return await _db.Activities.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(ActivityInfo activity)
    {
        await _db.Activities.AddAsync(activity);
        await _db.SaveChangesAsync();
    }

    public async Task<ActivityInfo?> RemoveByIdAsync(int id)
    {
        // Create aux entity just for the id
        var activity = new ActivityInfo { Id = id, Name = "Dummy", Path = "Dummy Path" };

        try
        {
            var deleted = _db.Activities.Remove(activity);
            if (deleted == null)
                return null;

            var rowsAffected = await _db.SaveChangesAsync();
            return (rowsAffected > 0) ? deleted.Entity : null; 
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }

    public async Task<ActivityInfo?> UpdateAsync(ActivityInfo activity)
    {
        var updated = _db.Update(activity);
        if (updated == null) 
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return (rowsAffected > 0) ? updated.Entity : null;
    }
}

using Microsoft.EntityFrameworkCore;
using Entities = Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.ValueObjects.Activity;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Entities.Activity>> GetAllAsync()
    {
        var objList = await _db.ActivityInfos.ToListAsync();
        List<Entities.Activity> result = new List<Entities.Activity>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Entities.Activity?> GetByIdAsync(int id)
    {
        var obj = await _db.ActivityInfos.FirstOrDefaultAsync(x => x.Id == id);
        return obj?.ToDomain();
    }

    public async Task<IEnumerable<Entities.Activity>> GetByCyclistIdAsync(int id)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == id);
        if (cyclist == null)
            throw new NoCyclistException();

        // Get activities
        var objList = await _db.ActivityInfos
            .Where(a => a.CyclistId == id)
            .ToListAsync();
        List<Entities.Activity> result = new List<Entities.Activity>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Entities.Activity> AddAsync(ActivitySummary summary, string path, int cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId);
        if (cyclist == null)
            throw new NoCyclistException();

        var saved = await _db.ActivityInfos.AddAsync(summary.ToModel(path, cyclistId));
        await _db.SaveChangesAsync();

        return saved.Entity.ToDomain();
    }

    public async Task<Entities.Activity?> RemoveByIdAsync(int id)
    {
        // Create aux entity just for the id
        var activity = new ActivityInfo { Id = id, CyclistId = 0, Name = "Dummy", Path = "Dummy Path" };

        try
        {
            var deleted = _db.ActivityInfos.Remove(activity);
            if (deleted == null)
                return null;

            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? deleted.Entity.ToDomain() : null; 
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }

    public async Task<Entities.Activity?> UpdateAsync(Entities.Activity activity)
    {
        var updated = _db.Update(activity.ToModel());
        if (updated == null) 
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }
}

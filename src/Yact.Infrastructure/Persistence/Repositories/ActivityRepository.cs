using Microsoft.EntityFrameworkCore;
using Entities = Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Infrastructure.Persistence.Models;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

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

    public async Task<Entities.Activity?> GetByIdAsync(ActivityId id)
    {
        var obj = await _db.ActivityInfos.FirstOrDefaultAsync(x => x.Id == id.Value);
        return obj?.ToDomain();
    }

    public async Task<IEnumerable<Entities.Activity>> GetByCyclistIdAsync(CyclistId id)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == id.Value);
        if (cyclist == null)
            throw new NoCyclistException();

        // Get activities
        var objList = await _db.ActivityInfos
            .Where(a => a.CyclistId == id.Value)
            .ToListAsync();
        List<Entities.Activity> result = new List<Entities.Activity>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Entities.Activity> AddAsync(Entities.Activity activity)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == activity.CyclistId.Value);
        if (cyclist == null)
            throw new NoCyclistException();

        var saved = await _db.ActivityInfos.AddAsync(activity.ToModel());
        await _db.SaveChangesAsync();

        return saved.Entity.ToDomain();
    }

    public async Task<Entities.Activity?> RemoveByIdAsync(ActivityId id)
    {
        // Create aux entity just for the id
        var activity = new ActivityInfo { Id = id.Value, CyclistId = Guid.Empty, Name = "Dummy", Path = "Dummy Path" };

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

using Microsoft.EntityFrameworkCore;
using Entities = Yact.Domain.Entities.Activity;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Domain.Exceptions.Cyclist;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Entities.ActivityInfo>> GetAllAsync()
    {
        var objList = await _db.ActivityInfos.ToListAsync();
        List<Entities.ActivityInfo> result = new List<Entities.ActivityInfo>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Entities.ActivityInfo?> GetByIdAsync(int id)
    {
        var obj = await _db.ActivityInfos.FirstOrDefaultAsync(x => x.Id == id);
        return obj?.ToDomain();
    }

    public async Task<IEnumerable<Entities.ActivityInfo>> GetByCyclistIdAsync(int id)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == id);
        if (cyclist == null)
            throw new NoCyclistException();

        // Get activities
        var objList = await _db.ActivityInfos
            .Where(a => a.CyclistId == id)
            .ToListAsync();
        List<Entities.ActivityInfo> result = new List<Entities.ActivityInfo>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<int> AddAsync(Entities.ActivityInfo activity, int cyclistId)
    {
        var saved = await _db.ActivityInfos.AddAsync(activity.ToModel(cyclistId));
        await _db.SaveChangesAsync();

        return saved.Entity.Id;
    }

    public async Task<Entities.ActivityInfo?> RemoveByIdAsync(int id)
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

    public async Task<Entities.ActivityInfo?> UpdateAsync(Entities.ActivityInfo activity)
    {
        var updated = _db.Update(activity.ToModel());
        if (updated == null) 
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }
}

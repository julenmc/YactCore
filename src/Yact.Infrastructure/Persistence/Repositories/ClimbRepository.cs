using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Infrastructure.Persistence.Models.Climb;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ClimbRepository : IClimbRepository
{
    private readonly AppDbContext _db;

    public ClimbRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ClimbData>> GetAllAsync()
    {
        var climbList = await _db.Climbs.ToListAsync();
        List<ClimbData> result = new List<ClimbData>();
        foreach (var climb in climbList)
        {
            result.Add(climb.ToDomain());
        }
        return result;
    }

    public async Task<ClimbData?> GetByIdAsync(int id)
    {
        var climb = await _db.Climbs.FirstOrDefaultAsync(c => c.Id == id);
        return climb?.ToDomain();
    }

    public async Task<IEnumerable<ClimbData>> GetByCoordinates(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax)
    {
        var climbList = await _db.Climbs
            .Where(c => (c.LatitudeInit >= latitudeMin && c.LatitudeInit <= latitudeMax) &&
            (c.LongitudeInit >= longitudeMin && c.LongitudeInit <= longitudeMax))
            .ToListAsync();

        List<ClimbData> result = new List<ClimbData>();
        foreach (var climb in climbList)
        {
            result.Add(climb.ToDomain());
        }
        return result;
    }

    public async Task<int> AddAsync(ClimbData climb)
    {
        var newClimb = await _db.Climbs.AddAsync(climb.ToModel());
        await _db.SaveChangesAsync();

        return newClimb.Entity.Id;
    }

    public async Task<int> RemoveByIdAsync(int id)
    {
        // Create aux entity just for the id
        var climb = new ClimbInfo { Id = id };

        try
        {
            var deleted = _db.Climbs.Remove(climb);
            if (deleted == null)
                return -1;

            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? deleted.Entity.ToDomain().Id : -1;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return -1;
        }
    }

    public async Task<int> UpdateAsync(ClimbData climb)
    {
        var updated = _db.Update(climb.ToModel());
        if (updated == null)
            return -1;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain().Id : -1;
    }

}

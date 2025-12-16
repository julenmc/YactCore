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

    public async Task<IEnumerable<ClimbData>> GetByCoordinatesAsync(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax)
    {
        var actualLatMin = Math.Min(latitudeMin, latitudeMax);
        var actualLatMax = Math.Max(latitudeMin, latitudeMax);
        var actualLonMin = Math.Min(longitudeMin, longitudeMax);
        var actualLonMax = Math.Max(longitudeMin, longitudeMax);

        var climbList = await _db.Climbs
            .Where(c => c.LatitudeInit >= actualLatMin && c.LatitudeInit <= actualLatMax &&
                        c.LongitudeInit >= actualLonMin && c.LongitudeInit <= actualLonMax)
            .ToListAsync();

        List<ClimbData> result = new List<ClimbData>();
        foreach (var climb in climbList)
        {
            result.Add(climb.ToDomain());
        }
        return result;
    }

    public async Task<ClimbData> AddAsync(ClimbData climb)
    {
        var newClimb = await _db.Climbs.AddAsync(climb.ToModel());
        await _db.SaveChangesAsync();

        return newClimb.Entity.ToDomain();
    }

    public async Task<ClimbData?> RemoveByIdAsync(int id)
    {
        // Create aux entity just for the id
        var climb = new ClimbInfo { Id = id };

        try
        {
            var deleted = _db.Climbs.Remove(climb);
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

    public async Task<ClimbData?> UpdateAsync(ClimbData climb)
    {
        var updated = _db.Update(climb.ToModel());
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }

}

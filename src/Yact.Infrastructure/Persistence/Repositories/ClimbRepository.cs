using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Climb;
using Yact.Infrastructure.Persistence.Data;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ClimbRepository : IClimbRepository
{
    private readonly WriteDbContext _db;

    public ClimbRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<Climb?> GetByIdAsync(ClimbId id)
    {
        return await _db.Climbs.FindAsync(id);
    }

    public async Task<IEnumerable<Climb>> GetByCoordinatesAsync(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax)
    {
        var actualLatMin = Math.Min(latitudeMin, latitudeMax);
        var actualLatMax = Math.Max(latitudeMin, latitudeMax);
        var actualLonMin = Math.Min(longitudeMin, longitudeMax);
        var actualLonMax = Math.Max(longitudeMin, longitudeMax);

        return await _db.Climbs
            .Where(c => c.Data.Coordinates.LatitudeInit >= actualLatMin && c.Data.Coordinates.LatitudeInit <= actualLatMax &&
                        c.Data.Coordinates.LongitudeInit >= actualLonMin && c.Data.Coordinates.LongitudeInit <= actualLonMax)
            .ToListAsync();
    }

    public async Task<Climb> AddAsync(Climb climb)
    {
        var saved =  await _db.Climbs.AddAsync(climb);
        await _db.SaveChangesAsync();

        return saved.Entity;
    }

    public async Task<Climb?> RemoveByIdAsync(ClimbId id)
    {
        try
        {
            var climb = await _db.Climbs.FindAsync(id);
            if (climb == null)
                return null;

            _db.Climbs.Remove(climb);
            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? climb : null;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }

    public async Task<Climb?> UpdateAsync(Climb climb)
    {
        var updated = _db.Update(climb);
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity : null;
    }
}

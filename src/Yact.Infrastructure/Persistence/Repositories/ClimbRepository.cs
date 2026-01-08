using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Infrastructure.Persistence.Models;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ClimbRepository : IClimbRepository
{
    private readonly AppDbContext _db;

    public ClimbRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Climb>> GetAllAsync()
    {
        var climbList = await _db.Climbs.ToListAsync();
        List<Climb> result = new List<Climb>();
        foreach (var climb in climbList)
        {
            result.Add(climb.ToDomain());
        }
        return result;
    }

    public async Task<Climb?> GetByIdAsync(ClimbId id)
    {
        var climb = await _db.Climbs.FirstOrDefaultAsync(c => c.Id == id.Value);
        return climb?.ToDomain();
    }

    public async Task<IEnumerable<Climb>> GetByCoordinatesAsync(float latitudeMin, float latitudeMax, float longitudeMin, float longitudeMax)
    {
        var actualLatMin = Math.Min(latitudeMin, latitudeMax);
        var actualLatMax = Math.Max(latitudeMin, latitudeMax);
        var actualLonMin = Math.Min(longitudeMin, longitudeMax);
        var actualLonMax = Math.Max(longitudeMin, longitudeMax);

        var climbList = await _db.Climbs
            .Where(c => c.LatitudeInit >= actualLatMin && c.LatitudeInit <= actualLatMax &&
                        c.LongitudeInit >= actualLonMin && c.LongitudeInit <= actualLonMax)
            .ToListAsync();

        List<Climb> result = new List<Climb>();
        foreach (var climb in climbList)
        {
            result.Add(climb.ToDomain());
        }
        return result;
    }

    public async Task<Climb> AddAsync(Climb climb)
    {
        var newClimb = await _db.Climbs.AddAsync(climb.ToModel());
        await _db.SaveChangesAsync();

        return newClimb.Entity.ToDomain();
    }

    public async Task<Climb?> RemoveByIdAsync(ClimbId id)
    {
        // Create aux entity just for the id
        var climb = new ClimbInfo { Id = id.Value };

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

    public async Task<Climb?> UpdateAsync(Climb climb)
    {
        var updated = _db.Update(climb.ToModel());
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }
}

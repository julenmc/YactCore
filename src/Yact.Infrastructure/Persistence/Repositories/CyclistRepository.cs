using Microsoft.EntityFrameworkCore;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Data;
using Entities = Yact.Domain.Entities;

namespace Yact.Infrastructure.Persistence.Repositories;

public class CyclistRepository : ICyclistRepository
{
    private readonly WriteDbContext _db;

    public CyclistRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<Entities.Cyclist?> GetByIdAsync(CyclistId id)
    {
        return await _db.Cyclists
            .Where(c => c.Id == id)
            .Include(c => c.FitnessHistory
                .OrderByDescending(f => f.UpdateDate)
                .Take(10))
            .FirstOrDefaultAsync();
    }

    public async Task<Entities.Cyclist?> AddAsync(Entities.Cyclist cyclist)
    {
        var entry = await _db.Cyclists.AddAsync(cyclist);
        await _db.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Entities.Cyclist?> RemoveByIdAsync(CyclistId id)
    {
        try
        {
            var cyclist = await _db.Cyclists.FindAsync(id);
            if (cyclist == null)
                return null;

            _db.Cyclists.Remove(cyclist);
            var rowsAffected = await _db.SaveChangesAsync();
            return rowsAffected > 0 ? cyclist : null;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Entity didn't exist
            return null;
        }
    }

    public async Task<Entities.Cyclist?> UpdateAsync(Entities.Cyclist cyclist)
    {
        var updated = _db.Update(cyclist);
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity : null;
    }
}

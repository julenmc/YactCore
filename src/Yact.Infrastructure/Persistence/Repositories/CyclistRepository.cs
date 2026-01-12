using Microsoft.EntityFrameworkCore;
using Yact.Domain.Repositories;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Entities = Yact.Domain.Entities;

namespace Yact.Infrastructure.Persistence.Repositories;

public class CyclistRepository : ICyclistRepository
{
    private readonly AppDbContext _db;

    public CyclistRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Entities.Cyclist>> GetAllAsync()
    {
        return await _db.Cyclists.ToListAsync();
    }

    public async Task<Entities.Cyclist?> GetByIdAsync(CyclistId id, int? gapDays = null)
    {
        if (gapDays == null)
        {
            return await _db.Cyclists
                .Where(c => c.Id == id)
                .Include("_fitnessHistory")
                .FirstOrDefaultAsync();
        }
        else
        {
            DateTime allowedDate = DateTime.UtcNow.AddDays((double)-gapDays);
            return await _db.Cyclists
                .Include("_fitnessHistory")
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

    public async Task<IEnumerable<Entities.Cyclist>> GetByLastName(string lastName)
    {
        return await _db.Cyclists
            .Where(c => c.LastName == lastName)
            .Include("_fitnessHistory")
            .ToListAsync();
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

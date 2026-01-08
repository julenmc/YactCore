using Entities = Yact.Domain.Entities.Cyclist;
using Yact.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Models;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Repositories;

public class CyclistRepository : ICyclistRepository
{
    private readonly AppDbContext _db;

    public CyclistRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Domain.Entities.Cyclist>> GetAllAsync()
    {
        var objList = await _db.CyclistInfos.ToListAsync();
        List<Domain.Entities.Cyclist> result = new List<Domain.Entities.Cyclist>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Domain.Entities.Cyclist?> GetByIdAsync(CyclistId id)
    {
        var obj = await _db.CyclistInfos
            .Where(x => x.Id == id.Value)
            .Select(c => new
            {
                Cyclist = c,
                LatestFitness = c.Fitnesss!
                    .OrderByDescending(f => f.UpdateDate)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (obj?.Cyclist != null && obj.LatestFitness != null)
        {
            obj.Cyclist.Fitnesss = new List<CyclistFitness> { obj.LatestFitness };
        }

        return obj?.Cyclist.ToDomain();
    }

    public async Task<Domain.Entities.Cyclist?> AddAsync(Domain.Entities.Cyclist cyclist)
    {
        var saved = await _db.CyclistInfos.AddAsync(cyclist.ToModel());
        await _db.SaveChangesAsync();
        return saved.Entity.ToDomain();
    }

    public async Task<Domain.Entities.Cyclist?> RemoveByIdAsync(CyclistId id)
    {
        // Create aux entity just for the id
        var cyclist = new CyclistInfo { Id = id.Value, Name = "dummy", LastName = "dummy" };

        try
        {
            var deleted = _db.CyclistInfos.Remove(cyclist);
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

    public async Task<Domain.Entities.Cyclist?> UpdateAsync(Domain.Entities.Cyclist cyclist)
    {
        var updated = _db.Update(cyclist.ToModel());
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }
}

using Entities = Yact.Domain.Entities.Cyclist;
using Yact.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Infrastructure.Persistence.Models.Cyclist;
using Yact.Domain.Repositories;

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
        var objList = await _db.CyclistInfos.ToListAsync();
        List<Entities.Cyclist> result = new List<Entities.Cyclist>();
        foreach (var item in objList)
        {
            result.Add(item.ToDomain());
        }
        return result;
    }

    public async Task<Entities.Cyclist?> GetByIdAsync(int id)
    {
        var obj = await _db.CyclistInfos.FirstOrDefaultAsync(x => x.Id == id);
        return obj?.ToDomain();
    }

    public async Task<Entities.Cyclist?> AddAsync(Entities.Cyclist cyclist)
    {
        var saved = await _db.CyclistInfos.AddAsync(cyclist.ToModel());
        await _db.SaveChangesAsync();
        return saved.Entity.ToDomain();
    }

    public async Task<Entities.Cyclist?> RemoveByIdAsync(int id)
    {
        // Create aux entity just for the id
        var cyclist = new CyclistInfo { Id = id, Name = "dummy", LastName = "dummy" };

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

    public async Task<Entities.Cyclist?> UpdateAsync(Entities.Cyclist cyclist)
    {
        var updated = _db.Update(cyclist.ToModel());
        if (updated == null)
            return null;

        var rowsAffected = await _db.SaveChangesAsync();
        return rowsAffected > 0 ? updated.Entity.ToDomain() : null;
    }
}

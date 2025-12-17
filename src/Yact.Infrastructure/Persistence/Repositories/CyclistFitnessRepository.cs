using Microsoft.EntityFrameworkCore;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Infrastructure.Persistence.Models.Cyclist;
using Entities = Yact.Domain.Entities.Cyclist;

namespace Yact.Infrastructure.Persistence.Repositories;

public class CyclistFitnessRepository : ICyclistFitnessRepository
{
    private readonly AppDbContext _db;

    public CyclistFitnessRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Entities.CyclistFitness>> GetFitnessByCyclistIdAsync(int cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId);
        if (cyclist == null)
            throw new NoCyclistException();

        var objList = await _db.CyclistFitnesses
            .Where(f => f.CyclistId == cyclistId)
            .OrderBy(f => f.UpdateDate)
            .ToListAsync();
        List<Entities.CyclistFitness> ret = new List<Entities.CyclistFitness>();
        foreach (var obj in objList)
        {
            ret.Add(obj.ToDomain());
        }
        return ret;
    }

    public async Task<Entities.CyclistFitness?> GetCyclistLatestFitnessAsync(int cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId);
        if (cyclist == null)
            throw new NoCyclistException();

        var obj = await _db.CyclistFitnesses
            .Where(f => f.CyclistId == cyclistId)
            .OrderByDescending(f => f.UpdateDate)
            .FirstOrDefaultAsync();
        return obj?.ToDomain();
    }

    public async Task<Entities.CyclistFitness> AddFitnessAsync(Entities.CyclistFitness fitness, int cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId);
        if (cyclist == null)
            throw new NoCyclistException();

        var fitnessModel = fitness.ToModel(cyclistId);
        var obj = await _db.CyclistFitnesses.AddAsync(fitnessModel);

        await _db.SaveChangesAsync();
        return obj.Entity.ToDomain();
    }

    public async Task<Entities.CyclistFitness?> DeleteFitnessAsync(int id)
    {
        // Create aux model just for the ID
        CyclistFitness aux = new CyclistFitness() {  Id = id , CyclistId = 0 };

        try
        {
            var deleted = _db.CyclistFitnesses.Remove(aux);
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
}

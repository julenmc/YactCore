using Microsoft.EntityFrameworkCore;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;
using Yact.Infrastructure.Persistence.Models;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Repositories;

public class CyclistFitnessRepository : ICyclistFitnessRepository
{
    private readonly AppDbContext _db;

    public CyclistFitnessRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Domain.Entities.CyclistFitness>> GetFitnessByCyclistIdAsync(CyclistId cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId.Value);
        if (cyclist == null)
            throw new NoCyclistException();

        // Get fitness data
        var fitnessData = await _db.CyclistFitnesses
            .Where(f => f.CyclistId == cyclistId.Value)
            .ToListAsync();

        return fitnessData.Select(f => f.ToDomain()).ToList();
    }

    public async Task<Domain.Entities.CyclistFitness?> GetCyclistLatestFitnessAsync(CyclistId cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId.Value);
        if (cyclist == null)
            throw new NoCyclistException();

        // Get latest fitness
        var fitness = await _db.CyclistFitnesses
            .Where(f => f.CyclistId == cyclistId.Value)
            .OrderByDescending(f => f.UpdateDate)
            .FirstOrDefaultAsync();

        return fitness?.ToDomain();
    }

    public async Task<Domain.Entities.CyclistFitness> AddFitnessAsync(Domain.Entities.CyclistFitness fitness, CyclistId cyclistId)
    {
        // Check if cyclist exists
        var cyclist = await _db.CyclistInfos.FirstOrDefaultAsync(c => c.Id == cyclistId.Value);
        if (cyclist == null)
            throw new NoCyclistException();

        var fitnessModel = fitness.ToModel();
        var obj = await _db.CyclistFitnesses.AddAsync(fitnessModel);

        await _db.SaveChangesAsync();
        return obj.Entity.ToDomain();
    }

    public async Task<Domain.Entities.CyclistFitness?> DeleteFitnessAsync(CyclistFitnessId id)
    {
        // Create aux model just for the ID
        CyclistFitness aux = new CyclistFitness() { Id = id.Value, CyclistId = Guid.Empty };

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

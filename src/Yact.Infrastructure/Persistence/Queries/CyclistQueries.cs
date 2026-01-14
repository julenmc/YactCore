using Microsoft.EntityFrameworkCore;
using Yact.Application.Interfaces.Repositories;
using Yact.Application.ReadModels.Cyclists;
using Yact.Domain.Exceptions.Cyclist;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;

namespace Yact.Infrastructure.Persistence.Queries;

public class CyclistQueries : ICyclistQueries
{
    private readonly ReadDbContext _db;
    public CyclistQueries(ReadDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CyclistBasicReadModel>> GetByLastNameAsync(string lastName)
    {
        var objList = await _db.Cyclists
            .Where(c => c.LastName == lastName)
            .ToListAsync();

        return objList.Select(c => c.ToBasicModel());
    }

    public async Task<CyclistAdvancedReadModel> GetByIdAsync(Guid id)
    {
        var cyclist = await _db.Cyclists
            .Where(c => c.Id == id)
            .Include(c => c.Fitnesses)
            .Include(c => c.Activities)
            .FirstOrDefaultAsync();
        if (cyclist == null)
            throw new CyclistNotFoundException(id);

        return cyclist.ToAdvancedModel();
    }
}

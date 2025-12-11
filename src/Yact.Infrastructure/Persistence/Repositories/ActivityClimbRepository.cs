using Microsoft.EntityFrameworkCore;
using Entities = Yact.Domain.Entities.Climb;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Infrastructure.Persistence.Mappers;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityClimbRepository : IActivityClimbRepository
{
    private readonly AppDbContext _db;

    public ActivityClimbRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Entities.ActivityClimb>> GetByActivityAsync(int activityId)
    {
        var climbs = await _db.ActivityClimbs
            .Where(a => a.ActivityId == activityId)
            .Include(a => a.Climb)
            .ToListAsync();

        var ret = new List<Entities.ActivityClimb>();
        foreach (var climb in climbs)
        {
            ret.Add(climb.ToDomain());
        }
        return ret;
    }

    public async Task<List<Entities.ActivityClimb>> GetByClimbAsync(int climbId)
    {
        var climbs = await _db.ActivityClimbs
            .Where(a => a.ClimbId == climbId)
            .Include(a => a.Climb)
            .ToListAsync();

        var ret = new List<Entities.ActivityClimb>();
        foreach (var climb in climbs)
        {
            ret.Add(climb.ToDomain());
        }
        return ret;
    }
}

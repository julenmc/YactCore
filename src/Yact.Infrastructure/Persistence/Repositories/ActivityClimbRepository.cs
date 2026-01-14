using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Persistence.Data;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Infrastructure.Persistence.Repositories;

public class ActivityClimbRepository : IActivityClimbRepository
{
    private readonly WriteDbContext _db;

    public ActivityClimbRepository(WriteDbContext db)
    {
        _db = db;
    }

    public async Task<List<ActivityClimb>> GetByActivityAsync(ActivityId activityId)
    {
        //var climbs = await _db.ActivityClimbs
        //    .Where(a => a.ActivityId == activityId.Value)
        //    .Include(a => a.Climb)
        //    .ToListAsync();

        //var ret = new List<ActivityClimb>();
        //foreach (var climb in climbs)
        //{
        //    ret.Add(climb.ToDomain());
        //}
        //return ret;
        throw new NotImplementedException();
    }

    public async Task<List<ActivityClimb>> GetByClimbAsync(ClimbId climbId)
    {
        //var climbs = await _db.ActivityClimbs
        //    .Where(a => a.ClimbId == climbId.Value)
        //    .Include(a => a.Activity)
        //    .ToListAsync();

        //var ret = new List<ActivityClimb>();
        //foreach (var climb in climbs)
        //{
        //    ret.Add(climb.ToDomain());
        //}
        //return ret;
        throw new NotImplementedException();
    }

    public async Task AddAsync(ActivityClimb activityClimb)
    {
        //await _db.ActivityClimbs.AddAsync(activityClimb.ToModel());
        //await _db.SaveChangesAsync();
        throw new NotImplementedException();
    }
}

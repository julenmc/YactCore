using Microsoft.EntityFrameworkCore;
using Yact.Domain.Models;
using Yact.Domain.Repositories;
using Yact.Infrastructure.Data;

namespace Yact.Infrastructure.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Activity>> GetAllAsync()
    {
        return await _db.Activities.ToListAsync();
    }

    public async Task<Activity?> GetByIdAsync(int id)
    {
        return await _db.Activities.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Activity activity)
    {
        await _db.Activities.AddAsync(activity);
        _db.SaveChanges();
    }
}

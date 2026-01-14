using Microsoft.EntityFrameworkCore;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Data;

public class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {

    }

    public DbSet<CyclistReadModel> Cyclists { get; set; }
    //public DbSet<CyclistFitness> Fitnesses { get; set; }
    public DbSet<ActivityReadModel> Activities { get; set; }
    public DbSet<ClimbReadModel> Climbs { get; set; }
    public DbSet<ActivityClimb> ActivityClimbs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

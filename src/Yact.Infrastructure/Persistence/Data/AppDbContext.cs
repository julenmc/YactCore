using Microsoft.EntityFrameworkCore;
using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Models.Analytics;
using Yact.Infrastructure.Persistence.Models.Climb;
using Yact.Infrastructure.Persistence.Models.Cyclist;

namespace Yact.Infrastructure.Persistence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CyclistInfo> CyclistInfos { get; set; }
    public DbSet<CyclistFitness> CyclistFitnesses { get; set; }
    public DbSet<ActivityInfo> ActivityInfos { get; set; }
    public DbSet<ClimbInfo> Climbs { get; set; }
    public DbSet<Interval> Intervals { get; set; }
    public DbSet<ActivityClimb> ActivityClimbs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relation between Activity info table and activity file table
        //modelBuilder.Entity<Activity>()
        //    .HasOne(a => a.ActivityFile)
        //    .WithOne(d => d.ActivityInfo)
        //    .HasForeignKey<Activity>(d => d.ActivityFileId);

        modelBuilder.Entity<CyclistInfo>().HasData(new CyclistInfo
        {
            Id = 1,
            Name = "Dummy",
            LastName = "Cyclist",
            BirthDate = DateTime.Now,
        });

        modelBuilder.Entity<ActivityInfo>().HasData(new ActivityInfo
        {
            Id = 1,
            CyclistId = 1,
            Name = "Dummy Activity",
            Path = "dummy_activity.fit",
            Description = "This is a dummy activity",
            StartDate = DateTime.Now.AddMinutes(-30),
            EndDate = DateTime.Now,
            DistanceMeters = 10000,
            ElevationMeters = 100,
            Type = "Cycling",
            CreateDate = DateTime.Now,
        });

        modelBuilder.Entity<ClimbInfo>().HasData(new ClimbInfo
        {
            Id = 1,
            Name = "Unknown",
            LongitudeInit = 0,
            LongitudeEnd = 0,
            LatitudeInit = 0,
            LatitudeEnd = 0,
            AltitudeInit = 0,
            AltitudeEnd = 0,
            Slope = 0,
            MaxSlope = 0,
            Elevation = 0,
            Validated = true
        });
    }
}

using Microsoft.EntityFrameworkCore;
using Yact.Domain.Entities.Activity;

namespace Yact.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    //public DbSet<Climb> Climbs { get; set; }
    public DbSet<ActivityInfo> Activities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relation between Activity info table and activity file table
        //modelBuilder.Entity<Activity>()
        //    .HasOne(a => a.ActivityFile)
        //    .WithOne(d => d.ActivityInfo)
        //    .HasForeignKey<Activity>(d => d.ActivityFileId);

        //modelBuilder.Entity<Climb>().HasData(new Climb
        //{
        //    Id = 1,
        //    Name = "Dummy Climb",
        //    Path = "",
        //    LongitudeInit = 0,
        //    LongitudeEnd = 0,
        //    LatitudeInit = 0,
        //    LatitudeEnd = 0,
        //    AltitudeInit = 0,
        //    AltitudeEnd = 0,
        //    InitRouteDistance = 0,
        //    EndRouteDistance = 0,
        //    Distance = 0,
        //    AverageSlope = 0,
        //    MaxSlope = 0,
        //    HeightDiff = 0
        //});

        modelBuilder.Entity<ActivityInfo>().HasData(new ActivityInfo
        {
            Id = 1,
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
    }
}

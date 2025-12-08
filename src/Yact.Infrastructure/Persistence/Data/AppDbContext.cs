using Microsoft.EntityFrameworkCore;
using Yact.Infrastructure.Persistence.Models.Activity;
using Yact.Infrastructure.Persistence.Models.Cyclist;

namespace Yact.Infrastructure.Persistence.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ActivityInfo> Activities { get; set; }
    public DbSet<CyclistInfo> CyclistInfos { get; set; }
    public DbSet<CyclistFitness> CyclistFitnesses { get; set; }

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
    }
}

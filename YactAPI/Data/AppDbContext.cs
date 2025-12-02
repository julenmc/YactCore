using Microsoft.EntityFrameworkCore;
using YactAPI.Models;

namespace YactAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Climb> Climbs { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Climb>().HasData(new Climb
            {
                Id = 1,
                Name = "Dummy Climb",
                Path = "",
                LongitudeInit = 0,
                LongitudeEnd = 0,
                LatitudeInit = 0,
                LatitudeEnd = 0,
                AltitudeInit = 0,
                AltitudeEnd = 0,
                InitRouteDistance = 0,
                EndRouteDistance = 0,
                Distance = 0,
                AverageSlope = 0,
                MaxSlope = 0,
                HeightDiff = 0
            });

            modelBuilder.Entity<Activity>().HasData(new Activity
            {
                Id = 1,
                Name = "Dummy Activity",
                Path = "dummy_activity.fit",
            });
        }
    }
}

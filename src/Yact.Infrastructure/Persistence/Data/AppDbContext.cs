using MediatR;
using Microsoft.EntityFrameworkCore;
using Yact.Application.Common;
using Yact.Domain.Primitives;
using Yact.Infrastructure.Persistence.Models;

namespace Yact.Infrastructure.Persistence.Data;

public class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<Domain.Entities.Cyclist> Cyclists { get; set; }
    //public DbSet<CyclistFitness> CyclistFitnesses { get; set; }
    //public DbSet<ActivityInfo> ActivityInfos { get; set; }
    public DbSet<Domain.Entities.Climb> Climbs { get; set; }
    //public DbSet<Interval> Intervals { get; set; }
    //public DbSet<ActivityClimb> ActivityClimbs { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // 1. Get events before saving
        var domainEvents = GetDomainEvents();

        // 2. Save in DB
        var result = await base.SaveChangesAsync(ct);

        // 3. If all good, publish events
        await PublishDomainEvents(domainEvents, ct);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        //var cyclistId = Guid.NewGuid();
        //var fitnessId = Guid.NewGuid();
        ////var activityId = Guid.NewGuid();
        ////var climbId = Guid.NewGuid();

        //modelBuilder.Entity<Cyclist>().HasData(new Cyclist
        //{
        //    Id = cyclistId,
        //    Name = "Dummy",
        //    LastName = "Cyclist",
        //    BirthDate = DateTime.UtcNow,
        //});

        //modelBuilder.Entity<CyclistFitness>().HasData(new CyclistFitness
        //{
        //    Id = fitnessId,
        //    CyclistId = cyclistId,
        //    UpdateDate = DateTime.UtcNow,
        //    Height = 180,
        //    Weight = 70,
        //    Ftp = 250,
        //    Vo2Max = 50,
        //    PowerZonesRaw = @"[
        //        {""lowLimit"": 0, ""highLimit"":150},
        //        {""lowLimit"": 151, ""highLimit"": 200},
        //        {""lowLimit"": 201, ""highLimit"": 250},
        //        {""lowLimit"": 251, ""highLimit"": 300},
        //        {""lowLimit"": 301, ""highLimit"": 350},
        //        {""lowLimit"": 351, ""highLimit"": 400},
        //        {""lowLimit"": 401, ""highLimit"": 500}
        //    ]",
        //});

        //modelBuilder.Entity<ActivityInfo>().HasData(new ActivityInfo
        //{
        //    Id = activityId,
        //    CyclistId = cyclistId,
        //    Name = "Dummy Activity",
        //    Path = "dummy_activity.fit",
        //    Description = "This is a dummy activity",
        //    StartDate = DateTime.UtcNow.AddMinutes(-30),
        //    EndDate = DateTime.UtcNow,
        //    DistanceMeters = 10000,
        //    ElevationMeters = 100,
        //    Type = "Cycling",
        //    CreateDate = DateTime.UtcNow,
        //});

        //modelBuilder.Entity<ClimbInfo>().HasData(new ClimbInfo
        //{
        //    Id = climbId,
        //    Name = "Unknown",
        //    LongitudeInit = 0,
        //    LongitudeEnd = 0,
        //    LatitudeInit = 0,
        //    LatitudeEnd = 0,
        //    AltitudeInit = 0,
        //    AltitudeEnd = 0,
        //    Slope = 0,
        //    MaxSlope = 0,
        //    Elevation = 0,
        //    Validated = true
        //});
    }

    private List<IDomainEvent> GetDomainEvents()
    {
        var domainEvents = new List<IDomainEvent>();

        // Search entities that have events
        var entities = ChangeTracker
            .Entries<IEntity>() 
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            domainEvents.AddRange(entity.DomainEvents);
            entity.ClearDomainEvents(); 
        }

        return domainEvents;
    }

    private async Task PublishDomainEvents(
        List<IDomainEvent> domainEvents,
        CancellationToken ct)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent);

            await _mediator.Publish(domainEvent, ct);
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Yact.Application.Common;
using Yact.Domain.Entities;
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
    public DbSet<Domain.Entities.Activity> Activities { get; set; }
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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasKey(x => x.Id);

        // Convert from Value Object to Guid
        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => ActivityId.From(value))
            .IsRequired();

        builder.Property(a => a.CyclistId)
            .HasConversion(
                id => id.Value,
                value => CyclistId.From(value))
            .IsRequired();

        builder.OwnsOne(a => a.Path, path =>
        {
            path.Property(p => p.Value).HasColumnName("Path").IsRequired();
        });

        builder.OwnsOne(a => a.Summary, summary =>
        {
            summary.Property(s => s.Name)
                .HasColumnName("Name")
                .IsRequired();
            summary.Property(s => s.Description).HasColumnName("Description");
            summary.Property(s => s.StartDate).HasColumnName("StartDate");
            summary.Property(s => s.EndDate).HasColumnName("EndDate");
            summary.Property(s => s.DistanceMeters).HasColumnName("DistanceMeters");
            summary.Property(s => s.ElevationMeters).HasColumnName("ElevationMeters");
            summary.Property(s => s.Type).HasColumnName("Type");
            summary.Property(s => s.CreateDate).HasColumnName("CreationDate");
            summary.Property(s => s.UpdateDate).HasColumnName("UpdateDate");
        });

        // FK
        builder.HasOne<Cyclist>()
            .WithMany()
            .HasForeignKey(a => a.CyclistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(c => c.Records);
    }
}

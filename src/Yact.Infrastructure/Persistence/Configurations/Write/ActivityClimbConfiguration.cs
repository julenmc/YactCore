using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.ActivityClimb;

namespace Yact.Infrastructure.Persistence.Configurations.Write;

public class ActivityClimbConfiguration : IEntityTypeConfiguration<ActivityClimb>
{
    public void Configure(EntityTypeBuilder<ActivityClimb> builder)
    {
        builder.ToTable("ActivityClimbs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => ActivityClimbId.From(value))
            .IsRequired();
        builder.Property(x => x.StartPointMeters)
            .HasColumnName("StartPointMeters")
            .IsRequired();
        
        // FKs
        builder.HasOne<Climb>()
            .WithMany()
            .HasForeignKey(x => x.ClimbId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(x => x.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

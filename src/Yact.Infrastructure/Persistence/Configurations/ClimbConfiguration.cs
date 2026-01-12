using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Infrastructure.Persistence.Configurations;

public class ClimbConfiguration : IEntityTypeConfiguration<Climb>
{
    public void Configure(EntityTypeBuilder<Climb> builder)
    {
        builder.ToTable("Climbs");

        builder.HasKey(x => x.Id);

        // Convert from Value Object to Guid
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => ClimbId.From(value))
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(c => c.Summary.Name)
            .HasMaxLength(200)
            .HasColumnName("Name")
            .IsRequired();

        // Metrics
        builder.Property(c => c.Data.Metrics.DistanceMeters)
            .HasColumnName("DistanceMeters")
            .IsRequired();
        builder.Property(c => c.Data.Metrics.Slope)
            .HasColumnName("Slope")
            .IsRequired();
        builder.Property(c => c.Data.Metrics.MaxSlope)
            .HasColumnName("MaxSlope")
            .IsRequired();
        builder.Property(c => c.Data.Metrics.NetElevationMeters)
            .HasColumnName("NetElevationMeters")
            .IsRequired();
        builder.Property(c => c.Data.Metrics.TotalElevationMeters)
            .HasColumnName("TotalElevationMeters")
            .IsRequired();

        // Coordinates
        builder.Property(c => c.Data.Coordinates.LongitudeInit)
            .HasColumnName("LongitudeInit")
            .IsRequired();
        builder.Property(c => c.Data.Coordinates.LongitudeEnd)
            .HasColumnName("LongitudeEnd")
            .IsRequired();
        builder.Property(c => c.Data.Coordinates.LatitudeInit)
            .HasColumnName("LatitudeInit")
            .IsRequired();
        builder.Property(c => c.Data.Coordinates.LatitudeEnd)
            .HasColumnName("LatitudeEnd")
            .IsRequired();
        builder.Property(c => c.Data.Coordinates.AltitudeInit)
            .HasColumnName("AltitudeInit")
            .IsRequired();
        builder.Property(c => c.Data.Coordinates.AltitudeEnd)
            .HasColumnName("AltitudeEnd")
            .IsRequired();

    }
}

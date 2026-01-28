using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Climb;

namespace Yact.Infrastructure.Persistence.Configurations.Write;

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

        builder.OwnsOne(c => c.Summary, summary =>
        {
            summary.Property(s => s.Name)
            .HasMaxLength(200)
            .HasColumnName("Name")
            .IsRequired();
        });

        builder.OwnsOne(c => c.Data, data =>
        {
            data.OwnsOne(d => d.Metrics, metrics =>
            {
                metrics.Property(m => m.DistanceMeters).HasColumnName("DistanceMeters").IsRequired();
                metrics.Property(m => m.Slope).HasColumnName("Slope").IsRequired();
                metrics.Property(m => m.MaxSlope).HasColumnName("MaxSlope").IsRequired();
                metrics.Property(m => m.NetElevationMeters).HasColumnName("NetElevationMeters").IsRequired();
                metrics.Property(m => m.TotalElevationMeters).HasColumnName("TotalElevationMeters").IsRequired();
            });
            data.OwnsOne(d => d.Coordinates, coordinates =>
            {
                coordinates.Property(c => c.LongitudeInit).HasColumnName("LongitudeInit").IsRequired();
                coordinates.Property(c => c.LongitudeEnd).HasColumnName("LongitudeEnd").IsRequired();
                coordinates.Property(c => c.LatitudeInit).HasColumnName("LatitudeInit").IsRequired();
                coordinates.Property(c => c.LatitudeEnd).HasColumnName("LatitudeEnd").IsRequired();
                coordinates.Property(c => c.AltitudeInit).HasColumnName("AltitudeInit").IsRequired();
                coordinates.Property(c => c.AltitudeEnd).HasColumnName("AltitudeEnd").IsRequired();
            });
            data.Property(d => d.StartPointMeters).HasColumnName("StartPointMeters");
        });    
    }
}

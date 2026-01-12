using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Configurations;

public class CyclistFitnessConfiguration : IEntityTypeConfiguration<CyclistFitness>
{
    public void Configure(EntityTypeBuilder<CyclistFitness> builder)
    {
        builder.ToTable("CyclistFitnesses");

        builder.HasKey(f => f.Id);

        // Convert IDs (Value Objects)
        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                value => CyclistFitnessId.From(value)
            )
            .HasColumnName("Id");

        builder.Property(f => f.CyclistId)
            .HasConversion(
                id => id.Value,
                value => CyclistId.From(value)
            )
            .IsRequired();

        builder.OwnsOne(f => f.Size, size =>
        {
            size.Property(s => s.HeightCm)
                .HasColumnName("Height")
                .IsRequired();

            size.Property(s => s.WeightKg)
                .HasColumnName("Weight")
                .IsRequired();
        });

        builder.Property(f => f.Ftp)
            .HasColumnName("FTP")
            .IsRequired();

        builder.Property(f => f.Vo2Max)
            .HasColumnName("VO2Max")
            .IsRequired();

        builder.Property(f => f.UpdateDate)
            .HasColumnName("UpdateDate")
            .IsRequired();

        builder.Property(f => f.PowerCurve)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<int, int>>(v, (JsonSerializerOptions?)null)!
            );

        builder.Property(f => f.HrZones)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<int, Zone>>(v, (JsonSerializerOptions?)null)!
            );

        builder.Property(f => f.PowerZones)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<int, Zone>>(v, (JsonSerializerOptions?)null)!
            );

        // Index to optimize search by dates
        builder.HasIndex("CyclistId", nameof(CyclistFitness.UpdateDate))
            .HasDatabaseName("IX_CyclistFitnesses_CyclistId_UpdateDate");
    }
}

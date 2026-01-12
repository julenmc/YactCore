using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;

namespace Yact.Infrastructure.Persistence.Configurations;

public class CyclistConfiguration : IEntityTypeConfiguration<Cyclist>
{
    public void Configure(EntityTypeBuilder<Cyclist> builder)
    {
        builder.ToTable("Cyclists");

        builder.HasKey(c => c.Id);

        // Convert from Value Object to Guid
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,                    // Domain → BD
                value => CyclistId.From(value)     // BD → Domain
            )
            .HasColumnName("Id")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(200)
            .IsRequired();

        // Relation with CyclistFitness
        builder.HasMany(c => c.FitnessHistory)
            .WithOne()
            .HasForeignKey("CyclistId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.LatestFitness);
        //builder.Ignore(c => c.FitnessHistory);
    }
}

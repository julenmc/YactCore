using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Configurations.Read;

public class CyclistReadConfiguration : IEntityTypeConfiguration<CyclistReadModel>
{
    public void Configure(EntityTypeBuilder<CyclistReadModel> builder)
    {
        builder.ToTable("Cyclists");

        builder.HasKey(x => x.Id);

        // Fitnesses and activities
        builder.HasMany(c => c.Fitnesses)
            .WithOne(f => f.Cyclist)
            .HasForeignKey(f => f.CyclistId);

        builder.HasMany(c => c.Activities)
            .WithOne(a => a.Cyclist)
            .HasForeignKey(a => a.CyclistId);
    }
}

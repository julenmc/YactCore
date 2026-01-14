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
            .WithOne();

        builder.HasMany(c => c.Activities)
            .WithOne();
    }
}

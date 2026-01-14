using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Configurations.Read;

public class CyclistFitnessReadConfiguration : IEntityTypeConfiguration<CyclistFitnessReadModel>
{
    public void Configure(EntityTypeBuilder<CyclistFitnessReadModel> builder)
    {
        builder.ToTable("CyclistFitnesses");

        builder.HasKey(x => x.Id);

        // FK of cyclist
        builder.HasOne(f => f.Cyclist)
            .WithMany()
            .HasForeignKey(f => f.CyclistId);
    }
}

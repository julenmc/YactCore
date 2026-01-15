using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Configurations.Read;

public class ActivityReadConfiguration : IEntityTypeConfiguration<ActivityReadModel>
{
    public void Configure(EntityTypeBuilder<ActivityReadModel> builder)
    {
        builder.ToTable("Activities");
        builder.HasKey(x => x.Id);

        // ActivityClimbs
        builder.HasMany(a => a.ActivityClimbs)
            .WithOne(x => x.Activity)
            .HasForeignKey(x => x.ActivityId);

        // FK of Cyclist
        builder.HasOne(a => a.Cyclist)
            .WithMany()
            .HasForeignKey(a => a.CyclistId);
    }
}

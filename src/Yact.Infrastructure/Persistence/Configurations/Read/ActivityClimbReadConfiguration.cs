using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Configurations.Read;

public class ActivityClimbReadConfiguration : IEntityTypeConfiguration<ActivityClimbReadModel>
{
    public void Configure(EntityTypeBuilder<ActivityClimbReadModel> builder)
    {
        builder.ToTable("ActivityClimbs");
        builder.HasKey(x => x.Id);

        // FKs
        builder.HasOne(x => x.Activity)
            .WithMany()
            .HasForeignKey(x => x.ActivityId);
        builder.HasOne(x => x.Climb)
            .WithMany()
            .HasForeignKey(x => x.ClimbId);
    }
}

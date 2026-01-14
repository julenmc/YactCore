using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Configurations.Read;

public class ClimbReadConfiguration : IEntityTypeConfiguration<ClimbReadModel>
{
    public void Configure(EntityTypeBuilder<ClimbReadModel> builder)
    {
        builder.ToTable("Climbs");

        builder.HasKey(x => x.Id);
    }
}

using Yact.Infrastructure.Persistence.Models.Cyclist;
using Entities = Yact.Domain.Entities.Cyclist;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistMapper
{
    internal static Entities.Cyclist ToDomain(this CyclistInfo model)
    {
        return new Entities.Cyclist()
        {
            Id = model.Id,
            Name = model.Name,
            LastName = model.LastName,
            BirthDate = model.BirthDate,
            FitnessData = model.Fitnesss?.FirstOrDefault()?.ToDomain(),
        };
    }

    internal static CyclistInfo ToModel(this Entities.Cyclist entity)
    {
        return new CyclistInfo()
        {
            Id = entity.Id,
            Name = entity.Name != null ? entity.Name : "Unknown",
            LastName = entity.LastName != null ? entity.LastName : "Unknown",
            BirthDate = entity.BirthDate,
        };
    }
}

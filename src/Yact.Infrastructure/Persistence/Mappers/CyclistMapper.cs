using Yact.Application.ReadModels.Cyclists;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class CyclistMapper
{
    internal static CyclistBasicReadModel ToBasicModel(this CyclistReadModel cyclist)
    {
        return new CyclistBasicReadModel
        {
            Id = cyclist.Id,
            Name = cyclist.Name,
            LastName = cyclist.LastName,
            BirthDate = cyclist.BirthDate
        };
    }

    internal static CyclistAdvancedReadModel ToAdvancedModel(this CyclistReadModel cyclist)
    {
        return new CyclistAdvancedReadModel()
        {
            Id = cyclist.Id,
            Name = cyclist.Name,
            LastName = cyclist.LastName,
            BirthDate = cyclist.BirthDate,
            FitnessHistory = cyclist.Fitnesses?.Select(f => f.ToModel()),
            Activities = cyclist.Activities?.Select(a => a.ToBasicModel())
        };
    }
}

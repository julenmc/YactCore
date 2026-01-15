using Yact.Application.ReadModels.Activities;
using Yact.Infrastructure.Persistence.ReadModels;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityMapper
{
    internal static ActivityAdvancedReadModel ToAdvancedModel(this ActivityReadModel activity)
    {
        return new ActivityAdvancedReadModel()
        {
            Id = activity.Id,
            CyclistId = activity.CyclistId,
            Name = activity.Name,
            Description = activity.Description,
            Path = activity.Path,
            StartDate = activity.StartDate,
            EndDate = activity.EndDate,
            DistanceMeters = activity.DistanceMeters,
            ElevationMeters = activity.ElevationMeters,
            Type = activity.Type,
            CreateDate = activity.CreationDate,
            UpdateDate = activity.UpdateDate,
            ActivityClimbs = activity.ActivityClimbs.Select(x => x.ToModel())
        };
    }

    internal static ActivityBasicReadModel ToBasicModel(this ActivityReadModel activity)
    {
        return new ActivityBasicReadModel()
        {
            Id = activity.Id,
            Name = activity.Name,
            Description = activity.Description,
            StartDate = activity.StartDate,
            DistanceMeters = activity.DistanceMeters,
            ElevationMeters = activity.ElevationMeters,
            Type = activity.Type,
        };
    }
}

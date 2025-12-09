using Yact.Infrastructure.Persistence.Models.Activity;
using Entities = Yact.Domain.Entities.Activity;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityMapper
{
    internal static Entities.ActivityInfo ToDomain(this ActivityInfo model)
    {
        return new Entities.ActivityInfo()
        {
            Id = model.Id,
            CyclistId = model.CyclistId,
            Name = model.Name,
            Description = model.Description,
            Path = model.Path,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            DistanceMeters = model.DistanceMeters,
            ElevationMeters = model.ElevationMeters,
            Type = model.Type,
            CreateDate = model.CreateDate,
            UpdateDate = model.UpdateDate,
        };
    }

    internal static ActivityInfo ToModel(this Entities.ActivityInfo entity, int? cyclistId = null)
    {
        return new ActivityInfo
        {
            Id = entity.Id,
            CyclistId = (cyclistId != null) ? (int)cyclistId : entity.CyclistId,
            Name = entity.Name,
            Description = entity.Description,
            Path = entity.Path,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            DistanceMeters = entity.DistanceMeters,
            ElevationMeters = entity.ElevationMeters,
            Type = entity.Type,
            CreateDate = entity.CreateDate,
            UpdateDate = entity.UpdateDate,
        };
    }
}

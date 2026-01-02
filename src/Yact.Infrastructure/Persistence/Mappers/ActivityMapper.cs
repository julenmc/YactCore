using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Models.Activity;
using Entities = Yact.Domain.Entities.Activity;
using VO = Yact.Domain.ValueObjects.Activity;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityMapper
{
    internal static Entities.Activity ToDomain(this ActivityInfo model)
    {
        var summary = VO.ActivitySummary.Create(
            name: model.Name ?? "Unknown",
            description: model.Description ?? "",
            startDate: model.StartDate,
            endDate: model.EndDate,
            distance: model.DistanceMeters,
            elevation: model.ElevationMeters,
            type: model.Type ?? "Unknown"
            //createDate: model.CreateDate,
            //updateDate: model.UpdateDate
        );
        return Entities.Activity.Create(
            VO.ActivityId.From(model.Id),
            CyclistId.From(model.CyclistId), 
            VO.FilePath.From(model.Path ?? "Unknown"), 
            summary);
    }

    internal static ActivityInfo ToModel(this VO.ActivitySummary summary, string path, int cyclistId)
    {
        return new ActivityInfo
        {
            CyclistId = (int)cyclistId,
            Name = summary.Name,
            Description = summary.Description,
            Path = path,
            StartDate = summary.StartDate,
            EndDate = summary.EndDate,
            DistanceMeters = summary.DistanceMeters,
            ElevationMeters = summary.ElevationMeters,
            Type = summary.Type,
        };
    }

    internal static ActivityInfo ToModel(this Entities.Activity entity)
    {
        var ret = ToModel(entity.Summary, entity.Path.Value, entity.CyclistId.Value);
        ret.Id = entity.Id.Value;
        return ret;
    }
}

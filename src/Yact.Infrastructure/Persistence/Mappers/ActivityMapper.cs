using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Models;
using Entities = Yact.Domain.Entities.Activity;
using VO = Yact.Domain.ValueObjects.Activity;

namespace Yact.Infrastructure.Persistence.Mappers;

internal static class ActivityMapper
{
    internal static Entities ToDomain(this ActivityInfo model)
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
        return Entities.Load(
            VO.ActivityId.From(model.Id),
            CyclistId.From(model.CyclistId), 
            VO.FilePath.From(model.Path ?? "Unknown"), 
            summary);
    }

    internal static ActivityInfo ToModel(this Entities activity)
    {
        return new ActivityInfo
        {
            Id = activity.Id.Value,
            CyclistId = activity.CyclistId.Value,
            Name = activity.Summary.Name,
            Description = activity.Summary.Description,
            Path = activity.Path.Value,
            StartDate = activity.Summary.StartDate,
            EndDate = activity.Summary.EndDate,
            DistanceMeters = activity.Summary.DistanceMeters,
            ElevationMeters = activity.Summary.ElevationMeters,
            Type = activity.Summary.Type,
        };
    }
}
